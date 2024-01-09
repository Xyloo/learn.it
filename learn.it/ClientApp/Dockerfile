FROM node:20-alpine AS builder
COPY ./learn.it/ClientApp/package.json ./learn.it/ClientApp/npm-shrinkwrap.json ./learn.it/ClientApp/angular.json ./
RUN npm i && mkdir /ng-app && cp -R ./node_modules ./ng-app
WORKDIR /ng-app
COPY ./learn.it/ClientApp .
RUN npm run build

FROM nginx:alpine
COPY ./nginx.conf /etc/nginx/conf.d/default.conf
RUN rm -rf /usr/share/nginx/html/*
## From ‘builder’ stage copy over the artifacts in dist folder to default nginx public folder
COPY --from=builder /ng-app/dist /usr/share/nginx/html