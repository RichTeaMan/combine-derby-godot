FROM barichello/godot-ci:mono-4.2.1

ENV GODOT_VERSION 4.2.1.stable.mono
ENV TEMPLATE_DIRECTORY /root/.local/share/godot/templates

RUN wget https://packages.microsoft.com/config/debian/10/packages-microsoft-prod.deb -O packages-microsoft-prod.deb &&\
    dpkg -i packages-microsoft-prod.deb &&\
    rm packages-microsoft-prod.deb &&\
    apt-get update &&\
    apt-get install -y dotnet-sdk-7.0

RUN mkdir -v -p ~/.local/share/godot/export_templates
RUN mv ${TEMPLATE_DIRECTORY}/${GODOT_VERSION} ~/.local/share/godot/export_templates/${GODOT_VERSION}

WORKDIR /proj
