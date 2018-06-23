FROM ubuntu:18.04

RUN apt-get update \
	&& apt-get -y install joe less gnupg ssh wget curl net-tools iputils-ping libncurses5-dev autoconf libncursesw5-dev git

# Set the timezone.
RUN ln -fs /usr/share/zoneinfo/UTC /etc/localtime

RUN apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
RUN echo "deb https://download.mono-project.com/repo/ubuntu stable-bionic main" | tee /etc/apt/sources.list.d/mono-official-stable.list

RUN apt-get -y install mono-devel

RUN mkdir -p /app
WORKDIR /app
RUN git clone https://github.com/SIPp/sipp.git
RUN cd sipp && ./build.sh

//git clone .....

COPY Factom-Monitor/bin/Release/* /app/

COPY entrypoint.sh /entrypoint.sh
RUN chmod +x entrypoint.sh


#ENTRYPOINT ["/bin/bash"]
ENTRYPOINT ["/entrypoint.sh"]