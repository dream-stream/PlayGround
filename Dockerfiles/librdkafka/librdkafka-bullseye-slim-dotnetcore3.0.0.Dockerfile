FROM dreamstream/dotnetcore:3.0.0-bullseye-slim

# Install librdkafka
# https://packages.debian.org/sid/armhf/librdkafka-dev
COPY librdkafka1_1.2.1-1_armhf.deb librdkafka++1_1.2.1-1_armhf.deb librdkafka-dev_1.2.1-1_armhf.deb ./

RUN apt-get install ./librdkafka1_1.2.1-1_armhf.deb
RUN apt-get install ./librdkafka++1_1.2.1-1_armhf.deb
RUN apt-get install ./librdkafka-dev_1.2.1-1_armhf.deb