FROM node:18 AS build

WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .

RUN npm run build --omit=dev

FROM nginx:alpine

COPY nginx.conf /etc/nginx/nginx.conf

COPY --from=build /app/dist/pregnancy-app-frontend /usr/share/nginx/html

CMD ["nginx", "-g", "daemon off;"]
