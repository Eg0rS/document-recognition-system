FROM python:3.8-slim

WORKDIR /app

COPY ./requirements.txt /app/requirements.txt

RUN apt-get update \
    && apt-get install gcc ffmpeg libsm6 libxext6 -y \
    && apt-get clean

RUN pip install -r /app/requirements.txt

COPY . /app/
ENTRYPOINT ["python", "main.py"]
