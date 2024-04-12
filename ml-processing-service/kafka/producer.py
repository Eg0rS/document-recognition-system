import asyncio

from aiokafka import AIOKafkaProducer

from config.kafka_config import KafkaConfig
from kafka.model.models import Response


class Producer:
    def __init__(self, config: KafkaConfig):
        self.config = config

    async def __send(self, response: Response):
        self.producer = AIOKafkaProducer(
            bootstrap_servers=self.config.get_host(),
            value_serializer=lambda v: v.encode('utf-8')
        )
        await self.producer.start()
        try:
            await self.producer.send_and_wait(self.config.get_producer_topic(), response)
        finally:
            await self.producer.stop()

    async def start_send(self, response: Response):
        asyncio.run(self.__send(response))
