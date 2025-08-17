start:
	dotnet run

dev:
	dotnet watch run

up:
	docker-compose up -d

down:
	docker-compose down --remove-orphans

db-update:
	dotnet ef database update