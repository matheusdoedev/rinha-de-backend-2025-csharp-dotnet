start:
	dotnet run

dev:
	dotnet watch run

up:
	docker-compose up -d

down:
	docker-compose down --remove-orphans

update:
	dotnet ef database update