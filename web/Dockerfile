FROM node:22-alpine AS build
WORKDIR /app
COPY package*.json ./
# Replace with `npm ci` after the initial package-lock.json is committed.
RUN npm install
COPY . .
RUN npm run build

FROM node:22-alpine
WORKDIR /app
COPY --from=build /app/.output ./.output
ENV HOST=0.0.0.0
ENV PORT=3000
EXPOSE 3000
CMD ["node", ".output/server/index.mjs"]
