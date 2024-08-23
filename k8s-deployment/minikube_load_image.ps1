# List all Docker images starting with "microservice-demo" and load them into Minikube
$images = docker images --format "{{.Repository}}:{{.Tag}}" | Select-String '^microservice-demo'

foreach ($image in $images) {
    Write-Host "Loading image: $($image.Line)"
    minikube image load $($image.Line)
}

Write-Host "All images starting with 'microservice-demo' have been loaded into Minikube."
