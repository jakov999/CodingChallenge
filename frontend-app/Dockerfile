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
COPY --from=build /app/dist/frontend-app /usr/share/nginx/html  # Updated to use 'browser' folder
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80