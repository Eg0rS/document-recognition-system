FROM golang:1.20 as builder

EXPOSE 80

WORKDIR /go/src/app
COPY . .
RUN chmod +x ./main.go

RUN go build -o app
CMD ./app