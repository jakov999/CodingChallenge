# Frontend Dockerfile
FROM node:18 AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build --prod

FROM nginx:alpine
COPY --from=build /app/dist/frontend-app/browser /usr/share/nginx/html

COPY nginx.conf /etc/nginx/conf.d

EXPOSE 4200
CMD ["nginx", "-g", "daemon off;"]