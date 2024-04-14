# Api системы распознования доументов 

## Доступ к API:
[Swagger API](https://techtitans.duckdns.org/swagger/index.html)

## Web приложение:
[front-five-hazel.vercel.app](https://front-five-hazel.vercel.app)

## Android + Windows клиенты:
- [Android Client](https://drive.google.com/file/d/1vEBREyDeKnM_4X5tN3AgVW0IHYywUB_F/view?usp=sharing)
- [Windows Client](https://drive.google.com/file/d/1plRWZzlqy5f6QRq0hSmUjmVYgBoBnNZg/view?usp=sharing)

## Методы Api:
- Метод проверки апи для жюри 
  // метод выполняется несколько секунд (у нас не такой мощный сервер)
  // пример запроса и ответа находится в папке doctest
 post /detect
 тело:
  {  "image": "string" }

  ответ:
  {"Type":"driver_license","Series":"99 10","Number":"876422","PageNumber":1,"Confidence":1.000000}
- Клиентские методы:
  - DocumentRecognition
  принимает уникальный идентификатор пользователя userId и base64 строку image 
  в ответ приходит код ответа 200, означающий что мы начали обрабатывать запрос на распознование документа
  - DocumentRecognition/test/{userid}
   тестовый набор захардокоженных данных
  - DocumentRecognition/{userid}
   запрос на получение истории всех распознований со всеми размеченными полями
  - DocumentRecognition/download/{guid}
   запрос на получение csv данных по guid - уникальынй идентификатор обработки изображения, получается по запросу на историю распознавания
  - File/{fileid}
    запрос на получение размеченного файла, для отображения результата обработки на клиенте 
  - Ping
    проверка работоспособности api 

## Deploy:
Разветрывание всей системы на сервере осуществляется командой 
docker-compose up --build
После чего необходимо 10 минут смотреть на бегущие строки на эране (хехе)
Зачем можно пользоваться сервсисом и разспозновать документы 

## Откуда у нас столько клиентов?
Мы использовали Kotlin Multiplatform и можем билдить под все платформы один проект (не без боли конечно)
Репозиторий с проектом:
[ТЫК](https://github.com/niksahn/GagarinHack)

*p.s*
*мы очень старались успеть все что запланировали, поэтому качество кода сильно пострадало, обычно мы не пишем sql запросы в контоллерах, но в этот раз пришлось*
