FROM elementsregistry.azurecr.io/elements/logging-linux-debian:latest

COPY content/install /app
COPY content/init /init

USER root
RUN chgrp -R elementsgroup /app/
RUN chmod -R g+wx /app/

USER elementsuser

WORKDIR /app

ENTRYPOINT ["/bin/bash", "/init/init.sh"]
CMD ["./${{ values.component_id }}"]