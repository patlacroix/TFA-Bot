FROM mono:5.16.0.179 AS build

ARG SIPP_BRANCH="v3.5.2"
ARG BOT_BRANCH="master"

ENV TZ=UTC

RUN set -xe && \
  groupadd -g 999 tfa-bot && \
  useradd -r -m -u 999 -g tfa-bot tfa-bot && \
  apt-get update && \
  apt-get --no-install-recommends -y install \
    autoconf \
    automake \
    build-essential \
    git \
    iputils-ping \
    libncurses5-dev \
    libncursesw5-dev \
    locales \
    locales-all \
    mtr-tiny \
    net-tools \
    ssh \
    wget && \
  apt-get autoremove --purge && \
  apt-get clean && \
  rm -rf /var/lib/apt/lists/* && \
  cd /home/tfa-bot && \
  git clone --branch ${SIPP_BRANCH} https://github.com/SIPp/sipp.git && \
  wget https://dist.nuget.org/win-x86-commandline/latest/nuget.exe && \
  git clone -b ${BOT_BRANCH} https://git.factoid.org/TFA/TFA-Bot.git && \
  cd /home/tfa-bot/sipp && \
  ./build.sh && \
  cd /home/tfa-bot//TFA-Bot && \
  mono ../nuget.exe restore TFA-Bot.sln && \
  msbuild -p:Configuration=Release -property:GitCommit=$(git rev-parse HEAD) TFA-Bot.sln && \
  ln -fs /usr/share/zoneinfo/${TZ} /etc/localtime && \
  ln -fs /home/tfa-bot /app && \
  cp /home/tfa-bot/TFA-Bot/entrypoint.sh / && \
  chown -R tfa-bot:tfa-bot /home/tfa-bot && \
  chmod +x /entrypoint.sh

USER tfa-bot

WORKDIR /

ENTRYPOINT ["/entrypoint.sh"]

# mono doesn't seem to respond to the SIGTERM, so...
STOPSIGNAL SIGKILL