# DevOps Portfolio Application

## Overview
This DevOps portfolio application showcases three microservices and their associated infrastructure components. Let's dive into the details:

1. **Microservices**:
   - **bugservice**: A .NET 8 minimal API REST service for handling bugs.
   - **userservice**: Another .NET 8 minimal API REST service focused on user management.
   - **frontend**: A React-based user interface for the entire system.

2. **Event-Driven Approach**:
   - RabbitMQ is employed as an event broker between the `bugservice` and `userservice`.
   - Demonstrates an event-driven architecture.

3. **Containerization and CI**:
   - All microservices are containerized using Docker.
   - Continuous Integration (CI) is handled by GitHub Actions.

4. **Azure Infrastructure**:
   - The entire infrastructure runs in Azure.
   - Managed by Terraform, it includes:
     - Azure Kubernetes Cluster (AKS) for orchestrating containers.
     - Two PostgreSQL databases for data storage.
     - Managed Prometheus and Grafana for monitoring.
     - Azure KeyVault for secret management.
     - Application Gateway serving as the ingress controller for AKS.

5. **Helm Charts and ArgoCD**:
   - Each microservice has its own Helm chart.
   - Deployment is managed by ArgoCD, ensuring streamlined workflows.
