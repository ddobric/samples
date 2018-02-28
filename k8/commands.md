az aks browse -n myK8sCluster -g K8-RG

kubectl delete -f mywebapi-deployment.yaml

kubectl create -f mywebapi-deployment.yaml --validate=false

