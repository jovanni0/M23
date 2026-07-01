FROM node:20-alpine AS build

WORKDIR /app
COPY M23.Monitor/package*.json ./
RUN npm install

COPY M23.Monitor/ .

ARG VITE_CONTROLLER_HTTP_URL
ARG VITE_CONTROLLER_WS_URL
ENV VITE_CONTROLLER_HTTP_URL=$VITE_CONTROLLER_HTTP_URL
ENV VITE_CONTROLLER_WS_URL=$VITE_CONTROLLER_WS_URL

RUN npm run build

FROM nginx:alpine
COPY --from=build /app/build /usr/share/nginx/html
COPY M23.Monitor/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80