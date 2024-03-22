# Guía de instalación del agente de Dynatrace en Kubernetes

Sigue estos pasos para instalar el agente de Dynatrace en tu clúster de Kubernetes:

1. **Crea un espacio de nombres para Dynatrace**

  Ejecuta el siguiente comando para crear un nuevo espacio de nombres llamado `dynatrace` en tu clúster de Kubernetes:

  ```bash
  kubectl create namespace dynatrace
  ```

2. **Instala el operador de Dynatrace**

  Aplica el archivo de configuración del operador de Dynatrace en tu clúster. Este archivo contiene todas las configuraciones necesarias para instalar y ejecutar el operador de Dynatrace.

  ```bash
  kubectl apply -f https://github.com/Dynatrace/dynatrace-operator/releases/download/v0.14.2/kubernetes.yaml
  ```

3. **Espera a que el operador de Dynatrace esté listo**

  Ejecuta el siguiente comando para esperar hasta que el pod del operador de Dynatrace esté listo. Este comando espera hasta que todos los pods que coinciden con el selector proporcionado estén listos.

  ```bash
  kubectl -n dynatrace wait pod --for=condition=ready --selector=app.kubernetes.io/name=dynatrace-operator,app.kubernetes.io/component=webhook --timeout=300s
  ```

4. **Instala el agente de Dynatrace**

  Finalmente, aplica el archivo de configuración `dynakube.yaml` para instalar el agente de Dynatrace en tu clúster. Asegúrate de que este archivo esté correctamente configurado con tus detalles de Dynatrace.

  ```bash
  kubectl apply -f dynakube.yaml
  ```

¡Eso es todo! Ahora deberías tener el agente de Dynatrace funcionando en tu clúster de Kubernetes.