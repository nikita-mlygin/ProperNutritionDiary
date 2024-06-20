# Use an official node image as the base image
FROM node:latest

# Set the working directory inside the container
WORKDIR /app

# Copy package.json and package-lock.json to the working directory
COPY src/Frontend/properNutritionDiary-frontend/package*.json ./

# Install dependencies
RUN npm install

# Copy the rest of the application files to the working directory
COPY src/Frontend/properNutritionDiary-frontend/ .

# Expose the port that Vite uses (default is 5173)
EXPOSE 5173

# Command to run Vite development server
CMD ["npm", "run", "dev"]
