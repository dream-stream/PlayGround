FROM golang:1.12.9-alpine3.10 AS base
RUN apk add build-base bash git
RUN git clone https://github.com/edenhill/librdkafka.git
WORKDIR librdkafka
RUN ./configure --prefix /usr && make && make install
RUN apk del build-base bash git
RUN rm -r /go/librdkafka

FROM base AS builder
RUN apk add git pkgconf build-base
WORKDIR /go/src/app
COPY . .
RUN go get -d -v ./...
RUN go build -o app

FROM base
WORKDIR /go/src/app
COPY --from=builder /go/src/app/app .
ENTRYPOINT ["./app"]