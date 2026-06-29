FROM node:20-alpine AS build
WORKDIR /app
COPY M23.Monitor/package*.json ./
RUN npm ci
COPY M23.Monitor/ ./
RUN npm run build

FROM node:20-alpine AS final
WORKDIR /app
COPY --from=build /app ./
EXPOSE 5173
# Rulăm direct serverul de dev cu --host, sau adaptorul de producție dacă e configurat. 
# Pentru simplitate în testare, rămânem pe varianta dev accesibilă global:
CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0"]

