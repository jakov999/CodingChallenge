# Frontend Dockerfile

# Build stage
FROM node:18-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build --prod

# Serve stage
FROM nginx:alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf  # Custom NGINX configuration
COPY --from=build /app/dist/frontend-app/browser /usr/share/nginx/html  # Updated to use 'browser' folder
EXPOSE 80