from ultralytics import YOLO
from PIL import Image
import time
import easyocr
import numpy as np
import cv2
import os
from dict_documents import *


# определение принадлежности дока к одному из классов и вывод на вероятности и имени класса
def classify_document(IMG_PATH):
    model = YOLO('../models/classify/best_classifier.pt')  # load a custom model
    # Predict with the model
    results = model(IMG_PATH,conf=0.8)  # predict on an image
    probs = results[0].probs.cuda()
    best_prob = probs.top1
    names = model.names

    return best_prob,names[probs.top1]

def from_eng_name_to_rus(IMG_PATH):
    probs,pred_class = classify_document(IMG_PATH)
    ru_class=""
    if(pred_class == "passport_address"):
        ru_class = "страница с пропиской"
    elif(pred_class == "passport_first_page"):
        ru_class = "страница персональных данных"
    elif(pred_class == "sts_ver1" or pred_class == "sts_ver2"):
        ru_class = "СТС"
    elif(pred_class == "vu_ver1" or pred_class == "vu_ver2"):
        ru_class = "водительские права"
    elif(pred_class == "pts"):
        ru_class = "ПТС"

    return probs,ru_class


#определение угла поворота изображения
def get_degree_rotation(IMG_PATH):
    model = YOLO('../models/classify/best_rotate.pt')  # load a custom model

    results = model(IMG_PATH,conf=0.8)  # predict on an image
    probs = results[0].probs.cuda()
    names = model.names

    return names[probs.top1]

#поворот изображения к нормали
def rotate_image_to_normal(IMG_PATH):
    #Original_Image = Image.open(IMG_PATH)
    degree = int(get_degree_rotation(IMG_PATH))
    if(degree != 0):
        if(degree== 180):
            rotated_image1 = IMG_PATH.transpose(Image.ROTATE_180)
        elif(degree == 90):
            rotated_image1 = IMG_PATH.rotate(-90, expand=True)
        elif(degree == 270):
            rotated_image1 = IMG_PATH.transpose(Image.ROTATE_90)
        return rotated_image1

    return IMG_PATH



#подача нормированного изображения на вход моделькам
def load_normal_image_to_models(doc_type):
    pred_model = YOLO('../models/segmentation/best_passport.pt')

    if(doc_type == "страница с пропиской" or doc_type == "страница персональных данных"):
        pred_model = YOLO('../models/segmentation/best_passport.pt')  # load a custom model
    elif(doc_type == "водительские права"):
        pred_model = YOLO('../models/segmentation/best_driver_license.pt')  # load a custom model
    elif (doc_type == "ПТС"):
        pred_model = YOLO('../models/segmentation/best_pts.pt')  # load a custom model
    elif (doc_type == "СТС"):
        pred_model = YOLO('../models/segmentation/best_sts.pt')  # load a custom model

    return pred_model

def get_text_from_img(IMG_PATH):
    _,doc_type = from_eng_name_to_rus(IMG_PATH)
    model = load_normal_image_to_models(doc_type)
    results = model(IMG_PATH,device="0")
    classes = []
    coordinates = []
    detect_class = []

    im = Image.open(IMG_PATH)
    names = model.names

    for r in results:
        for c in r.boxes.cls:
            cords = r.boxes[0].xyxy[0].tolist()
            classes.append(names[int(c)])
            coordinates.append(cords)
        r.show()

    ordered_classes, ordered_coordinates = order_arrays(list(names.values()), classes, coordinates)

    #count = 1 #для сохранениия вырезанных картинок
    for i,cls in enumerate(ordered_classes):
        if(cls in "pass_series_and_number"):
            #count = count + 1
            im1 = im.crop(ordered_coordinates[i])
            ser_num = get_series_and_number_pass(im1)
            ser_num = " ".join(ser_num)
            ordered_classes.remove(i)
            detect_class.append(ser_num)
        im1 = im.crop(ordered_coordinates[i])
        detect_class.append(get_data_from_docs(im1))

    print("Предсказанные классы: ", ordered_classes)
    print("Предсказанные коодринаты: ", ordered_coordinates)
    dictionary = dict({})

    dictionary = write_to_dict(detect_class, IMG_PATH)
    #im1.save(f"im_{count}.jpg")

    print("Заполненные данные: ", dictionary)

    #return classes,coordinates

#упорядочиваем значения предсказанного массива классов,относительно изначальных
def order_arrays(ordered_fields, unordered_values, coordinates):
    # Создаем словари для хранения упорядоченных значений и координат
    ordered_values = []
    ordered_coordinates = []

    # Множество для отслеживания уникальных значений
    seen_values = set()

    # Проходим по каждому полю из упорядоченных полей
    for field in ordered_fields:
        # Находим индекс поля в неупорядоченном списке значений
        if field in unordered_values:
            indexes = [i for i, f in enumerate(unordered_values) if f == field]
            for index in indexes:
                value = unordered_values[index]
                if value not in seen_values:  # Проверяем наличие значения в множестве
                    # Добавляем значение в список упорядоченных значений
                    ordered_values.append(value)
                    # Добавляем соответствующие координаты из списка координат
                    ordered_coordinates.append(coordinates[index])
                    # Добавляем значение в множество, чтобы избежать дубликатов
                    seen_values.add(value)

    return ordered_values, ordered_coordinates

def write_to_dict(classes, IMG_PATH):
    _, doc_type = from_eng_name_to_rus(IMG_PATH)
    if (doc_type == "страница с пропиской" or doc_type == "страница персональных данных"):
        for key, value in zip(passport.keys(), classes):
            passport[key] = value

        # Добавляем "Отсутствует" для оставшихся ключей в словаре
        for key in passport:
            if passport[key] == " ":
                passport[key] = "Отсутствует"

        return passport

    elif (doc_type == "водительские права"):
        for key, value in zip(driver_license.keys(), classes):
            driver_license[key] = value

        # Добавляем "Отсутствует" для оставшихся ключей в словаре
        for key in driver_license:
            if driver_license[key] == " ":
                driver_license[key] = "Отсутствует"

        return  driver_license

    elif (doc_type == "ПТС"):
        for key, value in zip(pts.keys(), classes):
            pts[key] = value

        # Добавляем "Отсутствует" для оставшихся ключей в словаре
        for key in pts:
            if pts[key] == " ":
                pts[key] = "Отсутствует"
        return pts

    elif (doc_type == "СТС"):
        for key, value in zip(sts.keys(), classes):
            sts[key] = value

        # Добавляем "Отсутствует" для оставшихся ключей в словаре
        for key in sts:
            if sts[key] == " ":
                sts[key] = "Отсутствует"
        return sts

def get_series_and_number_pass(image):
    reader = easyocr.Reader(['ru', 'en'])  # this needs to run only once to load the model into memory
    Original_Image = Image.open(image)
    rotated_image1 = Original_Image.transpose(Image.ROTATE_90)
    result = reader.readtext(np.asarray(rotated_image1), detail=0)
    print(result)
    return result

def get_data_from_docs(image):
    reader = easyocr.Reader(['ru', 'en'])  # this needs to run only once to load the model into memory
    rotated_image1 = rotate_image_to_normal(image)
    result = reader.readtext(np.asarray(rotated_image1), detail=0)
    print(result)
    return result

def main():
    IMG_PATH = "../dataset/passport_3_jpg.rf.3eaddb37afa7548926fe7e23fbf2dfac.jpg"
    probs,get_class = from_eng_name_to_rus(IMG_PATH) #возвращает класс предсказанного дока и вероятность
    get_text_from_img(IMG_PATH)


if __name__ == '__main__':
    start_time = time.time()
    main()
    print("--- %s seconds ---" % (time.time() - start_time))