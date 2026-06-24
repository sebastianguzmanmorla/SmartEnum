# Cargar variables desde .env si existe
ifneq ("$(wildcard .env)","")
    include .env
endif

# Variables
SOLUTION_PATH=SebastianGuzmanMorla.SmartEnum.slnx
CORE_PROJECT=src/SebastianGuzmanMorla.SmartEnum/SebastianGuzmanMorla.SmartEnum.csproj
EF_PROJECT=src/SebastianGuzmanMorla.SmartEnum.EntityFrameworkCore/SebastianGuzmanMorla.SmartEnum.EntityFrameworkCore.csproj
PACK_OUTPUT=artifacts
NUGET_SOURCE=https://api.nuget.org/v3/index.json

# Comandos dependientes del sistema operativo
ifeq ($(OS),Windows_NT)
    RM_DIR = if exist "$(subst /,\,$(PACK_OUTPUT))" rmdir /s /q "$(subst /,\,$(PACK_OUTPUT))"
else
    RM_DIR = rm -rf $(PACK_OUTPUT)
endif

.PHONY: clean pack push check-env

check-env:
ifndef API_KEY
	$(error API_KEY no encontrada. Asegurate de tener un archivo .env con API_KEY=xxx)
endif

clean:
	@echo "Limpiando binarios..."
	@-$(RM_DIR)
	@dotnet clean $(SOLUTION_PATH) -c Release

build:
	@echo "Compilando solución SmartEnum..."
	@dotnet build $(SOLUTION_PATH) -c Release

pack: clean
	@echo "Empaquetando proyectos..."
	@dotnet pack $(CORE_PROJECT) -c Release -o $(PACK_OUTPUT)
	@dotnet pack $(EF_PROJECT) -c Release -o $(PACK_OUTPUT)

push: check-env pack
	@echo "Publicando en NuGet..."
	@dotnet nuget push $(PACK_OUTPUT)/*.nupkg \
		--api-key $(API_KEY) \
		--source $(NUGET_SOURCE) \
		--skip-duplicate