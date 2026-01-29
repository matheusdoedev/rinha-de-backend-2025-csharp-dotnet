start:
	dotnet run

dev:
	dotnet watch run

up:
	docker-compose up -d

up-production:
	docker-compose -f ./docker-compose.production.yml up -d --remove-orphans

down-production:
	docker-compose -f ./docker-compose.production.yml down

down:
	docker-compose down --remove-orphans

db-update:
	dotnet ef database update

deploy:
	docker build -t matheusdoe/rinha-backend:latest .
	docker push matheusdoe/rinha-backend:latest