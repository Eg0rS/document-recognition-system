import asyncio
import requests
import torch

import base64
import io
import cv2
import os
import numpy as np
import json
import easyocr

from aiokafka import AIOKafkaConsumer
from config.kafka_config import KafkaConfig
from aiokafka import AIOKafkaProducer

from models_activities import convert_predict_class_en, rotate_image_to_normal, classify_document, get_model, get_images_text, get_plotted_image

from ultralytics import YOLO
from PIL import Image
from enum import Enum

class Consumer:
    def __init__(self, config: KafkaConfig):
        self.config = config

    def get_file_image(self, file_id):
        r = requests.get('http://' + 'file-service' + ':' + '10002' + '/api/files/' + file_id)
        # print(r.content)
        image_data = base64.b64encode(r.content)
        base64_decoded = base64.b64decode(image_data)
        image = Image.open(io.BytesIO(base64_decoded))
        return image

    def inference(self, image):
        model = YOLO('models/best_driver_license.pt')
        results = model.predict(image)
        # probs = ""

        res_plotted = results[0].plot()
        img = Image.fromarray(res_plotted)

        # for result in results:
        #     res_plotted = result.plot()
        #     img = Image.fromarray(res_plotted)
            # image.save('output.png')
        #     for c in range(0, 5):
        #         id = result.probs.top5[c]
        #         prob = str(id) + " " + result.names[id] + " " + "%f" % result.probs.top5conf.numpy()[c] + "\r\n"
        #         probs += prob

        return results[0]

    def post_image(self, image):
        buffered = io.BytesIO()
        image.save(buffered, format='png')
        img_encoded = base64.b64encode(buffered.getvalue())
        img_str = img_encoded.decode('ascii')
        json_data = json.dumps({'image': img_str})
        r = requests.post('http://' + 'file-service' + ':' + '10002' + '/api/files', data=json_data)
        return r.text

    def post_file(self, result, source_img, image_guid):
        reader = easyocr.Reader(['en', 'ru'])
        boxes = result.boxes
        names = result.names

        dict = {}

        for i in range(len(boxes.cls.tolist())):
            crop_cls = names[boxes.cls.tolist()[i]]
            image_cropped = source_img.crop(boxes[i].xyxy[0].tolist())

            buffered = io.BytesIO()
            image_cropped.save(buffered, format='png')
            img_byte_arr = buffered.getvalue()

            text_results = reader.readtext(img_byte_arr)
            total_text = ""
            for (_, text, _) in text_results:
                total_text += text + " "

            total_text = total_text.strip()
            if total_text != '':
                if crop_cls not in dict:
                    dict[crop_cls] = total_text
                else:
                    dict[crop_cls] += " " + total_text

        res_plotted = result.plot()
        image = Image.fromarray(res_plotted)
        probs = result.probs

        buffered = io.BytesIO()
        image.save(buffered, format='png')
        img_encoded = base64.b64encode(buffered.getvalue())
        img_str = img_encoded.decode('ascii')
        json_data = json.dumps({'image': img_str})
        r = requests.post('http://' + 'file-service' + ':' + '10002' + '/api/files', data=json_data)

        kafka_message = {
            'Guid': image_guid,
            'FileId': r.text,
            # 'Confidence': "%f" % probs.top1conf.numpy(),
            # 'Type': document_types[result.names[probs.top1]],
            'Series': 'nope',
            'Number': 'nope',
            'PageNumber': 1,
            'OptionalFields': dict
        }

        json_data_kafka = json.dumps(str(kafka_message))

        return json_data_kafka

    async def __consume(self):
        self.consumer = AIOKafkaConsumer(
            self.config.get_consumer_topic(),
            bootstrap_servers=self.config.get_host() + ':' + self.config.get_port(),
            auto_offset_reset='earliest',
            enable_auto_commit=True,
            group_id='request-consumer-group',
            auto_commit_interval_ms=200000,
            value_deserializer=lambda x: x.decode('utf-8')
        )

        producer = AIOKafkaProducer(
            bootstrap_servers=self.config.get_host() + ":" + self.config.get_port(),
            value_serializer=lambda x: json.dumps(x).encode('utf-8')
        )

        await self.consumer.start()
        await producer.start()
        print("consumer started")

        try:
            async for msg in self.consumer:
                print("consumed: ", msg.topic, msg.partition, msg.offset,
                      msg.key, msg.value, msg.timestamp)
                request = json.loads(msg.value)
                image = self.get_file_image(request['FileId'])
                top_prob, cls_name = classify_document(image)
                cls_name = convert_predict_class_en(cls_name)
                seg_model = get_model(cls_name)
                inf_result, texts, series, number = get_images_text(seg_model, image, cls_name)
                plotted_image = get_plotted_image(inf_result)
                post_result = self.post_image(plotted_image)

                kafka_message = {
                    'Guid': request['Guid'],
                    'FileId': post_result,
                    'Confidence': top_prob,
                    'Type': cls_name,
                    'Series': series,
                    'Number': number,
                    'PageNumber': 0,
                    'OptionalFields': texts
                }

                # inference_result = self.inference(image)
                # kafka_msg = self.post_file(inference_result, image, msg.value)

                await producer.send("resolutions", kafka_message)
        finally:
            print("consumer stopper")
            await producer.stop()
            await self.consumer.stop()

    def start_consume(self):
        asyncio.run(self.__consume())
