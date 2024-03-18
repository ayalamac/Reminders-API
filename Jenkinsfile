// * PoC CICD - Reminders-API
// * ************************************************************* //

//   Authors:
//   Sara Herrera       - sherrerar2@eafit.edu.co
//   Andrés Ayala       - aayalac@eafit.edu.co
//   Juan David Perdomo - jdperdomoq@eafit.edu.co
//   Santiago Patiño    - spatinob1@eafit.edu.co

pipeline {
    agent {
        kubernetes {
            idleMinutes 1
            yaml GetKubernetesAgent(['agentName': 'dotnet-sdk', 'version': 8])
        }
    }
    environment {

          // Agents
        String DOTNET_SDK_AGENT = 'dotnet-sdk-8'

          // Project
        String APP_NAME              = 'Reminders-API'
        String PROJECT_NAME          = "${APP_NAME}"
        String SONAR_QUALITY_LEVEL   = 'Standard'

          // File and Paths
        String SOLUTION_NAME                 = 'CleanArchitecture.sln'
        String PROJECT_UI_FOLDER             = 'src/CleanArchitecture.Api'
        String UNIT_TESTS_DOMAIN_FOLDER      = 'tests/CleanArchitecture.Domain.UnitTests'
        String UNIT_TESTS_APPLICATION_FOLDER = 'tests/CleanArchitecture.Application.UnitTests'
        String SUBCUTANEOUS_TESTS_FOLDER     = 'tests/CleanArchitecture.Application.SubcutaneousTests'
        String INTEGRATION_TESTS_FOLDER      = 'tests/CleanArchitecture.Api.IntegrationTests'

          // Destination
        String DEPLOY_ENVIRONMENT = "${BRANCH_NAME}"
        String TARGET_VERSION     = 'net8.0'
        String TARGET_PLATFORM    = 'win-x64'
        String PUBLISH_PATH       = "${PROJECT_UI_FOLDER}/bin/Release/${TARGET_VERSION}/${TARGET_PLATFORM}/publish"
        String ARTIFACT_NAME      = "reminders-api-artifact[${BRANCH_NAME.replaceAll('/', '-')}]#${BUILD_NUMBER}.zip"
    }
    options {
        skipStagesAfterUnstable()
        disableConcurrentBuilds()
    }

    stages {

        stage('Configuration') {
            parallel {
                stage('Restore dependencies') {
                    // Descargar y restaurar las dependencias necesarias para la construcción del proyecto.
                    steps {
                        container(DOTNET_SDK_AGENT) {
                            PrintHeader(['number': '1', 'title': 'Restore dependencies'])
                            script {
                                sh "dotnet restore ${SOLUTION_NAME} --no-cache --force -s https://api.nuget.org/v3/index.json"
                                sh "dotnet build"
                            }
                        }
                    }
                }

                // TODO : Configurar texto de ambientes
                stage('Update settings') {
                    // Actualizar las configuraciones necesarias antes de la compilación.
                    when { expression { return false } }
                    // when { expression { BRANCH_NAME in ['dev', 'qa', 'main', MAINTENANCE_BRANCH] } }
                    steps{
                        container(DOTNET_SDK_AGENT) {
                            PrintHeader(['number': '2', 'title': 'Update settings'])
                            script{
                                if (BRANCH_NAME == MAINTENANCE_BRANCH) {
                                    DEPLOY_ENVIRONMENT = 'dev'
                                }

                                Map currentEnvironment = [
                                    'dev': 'Desarrollo',
                                    'qa': 'Pruebas',
                                    'main': 'Producción'
                                ]

                                contentReplace(
                                    configs: [ 
                                        fileContentReplaceConfig(
                                            configs: [
                                                fileContentReplaceItemConfig( search: '(Local)', replace: currentEnvironment[DEPLOY_ENVIRONMENT], matchCount: 1),
                                            ],
                                            fileEncoding: 'UTF-8', filePath: "${PROJECT_UI_FOLDER}/appsettings.json")
                                    ]
                                )

                                
                            }
                        }
                    }
                }

                
            }
        }

        
        stage('Run security checks') {
            // Ejecutar pruebas y controles de seguridad para garantizar la integridad del código.
            parallel {
                stage('Git leaks'){
                    // Realizar una búsqueda en el código fuente para encontrar posibles fugas de información en el repositorio Git.
                    steps {
                        PrintHeader(['number': '3', 'title': 'SECURITY TOOL 1 - Git Leaks Check'])
                        build job: "GitLeaks.Tool",
                            parameters: [
                                string(name: 'PROJECT_GIT_URL', value: "${GIT_URL}"),
                                string(name: 'PROJECT_BRANCH_NAME', value: "${BRANCH_NAME}")
                            ],
                            propagate : true
                    }
                }

                stage('OWASP Dependency Check') {
                    // Verificar si hay dependencias vulnerables que pueden afectar la seguridad del proyecto.
                    agent {
                        kubernetes {
                            idleMinutes 1
                            yaml GetKubernetesPodYaml(['podName': 'owasp-dependency-check'])
                        }
                    }
                    steps {
                        container('owasp-dependency-check') {
                            PrintHeader(['number': '3', 'title': 'SECURITY TOOL 2 - OWASP Dependency Check'])
                            script{
                                RunDependencyCheck(['validateCache': true])
                            }
                        }
                    }
                }

                stage('SBOM for Dependency Track') {
                    // Crear un inventario de software que se utiliza en el proyecto.
                    steps {
                        PrintHeader(['number': '3', 'title': 'SECURITY TOOL 3 - Dependency Track - SBOM Creation and Publishing'])
                        build job: "DependencyTrack-DotNET.Tool",
                            parameters: [
                                string(name: 'PROJECT_NAME', value: "${PROJECT_NAME}"),
                                string(name: 'PROJECT_GIT_URL', value: "${GIT_URL}")
                            ],
                            wait: false,
                            propagate : false
                    }
                }
            }
        }

        stage('Tests Suite') {
            // Ejecutar pruebas unitarias y de integración para garantizar la calidad del desarrollo.
            parallel {
                stage('Unit Tests - Domain') {
                    // Ejecutar pruebas unitarias.
                    steps {
                        container(DOTNET_SDK_AGENT) {
                            PrintHeader(['number': '4', 'title': 'Unit tests - Domain'])
                            dir (UNIT_TESTS_DOMAIN_FOLDER) {
                                sh "dotnet test --no-build"
                            }
                        }
                    }
                }

                stage('Unit Tests - Application') {
                    // Ejecutar pruebas unitarias.
                    steps {
                        container(DOTNET_SDK_AGENT) {
                            PrintHeader(['number': '4', 'title': 'Unit tests - Application'])
                            dir (UNIT_TESTS_APPLICATION_FOLDER) {
                                sh "dotnet test --no-build"
                            }
                        }
                    }
                }

                stage('Subcutaneous Tests - Application') {
                    // Ejecutar pruebas subcutáneas.
                    steps {
                        container(DOTNET_SDK_AGENT) {
                            PrintHeader(['number': '4', 'title': 'Subcutaneous Tests - Application'])
                            dir (SUBCUTANEOUS_TESTS_FOLDER) {
                                sh "dotnet test --no-build"
                            }
                        }
                    }
                }

                stage('Integration Tests - Application') {
                    // Ejecutar pruebas subcutáneas.
                    steps {
                        container(DOTNET_SDK_AGENT) {
                            PrintHeader(['number': '4', 'title': 'Integration Tests - Application'])
                            dir (INTEGRATION_TESTS_FOLDER) {
                                sh "dotnet test --no-build"
                            }
                        }
                    }
                }
            }
        }

        stage('Sonarqube (SAST)') {
            // Configurar el proyecto en Sonarqube con el nivel de calidad requerido.
            // Realizar un análisis estático de seguridad del código y un análisis de calidad utilizando Sonarqube.
            steps {
                container(DOTNET_SDK_AGENT) {
                    PrintHeader(['number': '5', 'title': 'Static Application Security Testing (SAST) - Sonarqube'])
                    SetupSonarProject(['projectName': PROJECT_NAME, 'technology': TARGET_VERSION,  'qualityGateTier': SONAR_QUALITY_LEVEL])
                    RunSonarForDotnet(['projectName': PROJECT_NAME, 'solutionName': SOLUTION_NAME, 'branchName': BRANCH_NAME])
                }
            }
        }

        stage('Sonarqube result') {
            // Analizar los resultados de Sonarqube y tomar medidas para mejorar la seguridad del proyecto.
            steps {
                container(DOTNET_SDK_AGENT) {
                    PrintHeader(['number': '6', 'title': 'Sonarqube result'])
                    AssessSonarQualityCheck()
                }
            }
        }

        stage('Publish project') {
            // Compilar y publicar el proyecto para tener disponible el artefacto.
            // when { expression { DEPLOY_ENVIRONMENT in ['dev', 'qa', 'main'] } }
            when { expression { return false } }
            environment {
                String PUBLISH_MODE = 'Release'
            }
            steps {
                container(DOTNET_SDK_AGENT) {
                    PrintHeader(['number': '7', 'title': 'Publish project'])
                    script {
                        dir (PROJECT_UI_FOLDER) {
                            sh "dotnet publish -c ${PUBLISH_MODE} -r ${TARGET_PLATFORM} --self-contained false"
                        }

                        if (DEPLOY_ENVIRONMENT in ['main']) {
                            sh "apk update && apk upgrade && apk add --no-cache git"
                            String latestReleaseVersion = "v${GetAppVersionFromBranch()}"
                            Number repositoryId         = GetGitlabRepositoryId(['projectName': PROJECT_NAME])
                            CreateTagInGitlabRepository(['repositoryId': repositoryId, 'ref': 'main', 'tagName': latestReleaseVersion])

                            SendNotificationToDynatrace(['event': 'BUILD_FINISHED', 'entityId': "${DYNATRACE_PGI_ENTITY}", 'releaseVersion': latestReleaseVersion])
                        }
                    }
                }
            }
        }

        // stage('Archive and stash artifact') {
        //     // Crear un archivo del artefacto generado para futuras referencias.
        //     when { expression { DEPLOY_ENVIRONMENT in ['dev', 'qa', 'main'] } }
        //     steps {
        //         container(DOTNET_SDK_AGENT) {
        //             PrintHeader(['number': '8', 'title': 'Archive artifact'])
        //             script {
        //                 zip zipFile: "${ARTIFACT_NAME}", dir: "${PUBLISH_PATH}", overwrite: true
        //                 archiveArtifacts artifacts: "${ARTIFACT_NAME}", followSymlinks: false, onlyIfSuccessful: true
        //                 stash includes: "${ARTIFACT_NAME}", name: 'artifact'
        //             }
        //         }
        //     }
        // }

        // stage('Deploy') {
        //     // Desplegar el artefacto en el servidor.
        //     agent { label '.net' }
        //     options { skipDefaultCheckout() }
        //     when { expression { DEPLOY_ENVIRONMENT in ['dev', 'qa'] } }
        //     environment {
        //         String IIS_WEB_APPLICATION_NAME = "Default Web Site"
        //         String WEB_APPLICATION_IIS_SUBSITE = "${APP_NAME}Service"
                
        //         String DEV_SERVER = "srvwindllo1"

        //         String DEPLOY_SERVER = ''
        //         String APP_POOL_NAME = ''
        //         String IIS_APP_PATH = ''
        //     }
        //     steps {
        //         PrintHeader(['number': '9', 'title': 'Deploy'])
        //         script {
        //             unstash 'artifact'
        //             unzip zipFile: "${ARTIFACT_NAME}", dir: "${PUBLISH_PATH}"
        //             zip zipFile: 'temp-artifact.zip', dir: "${PUBLISH_PATH}", overwrite: true, glob: '*.*'

        //             DEPLOY_SERVER = DEV_SERVER
        //             APP_POOL_NAME = "${WEB_APPLICATION_IIS_SUBSITE}_${DEPLOY_ENVIRONMENT}"
        //             IIS_APP_PATH = "${DEPLOY_ENVIRONMENT}/${APP_POOL_NAME}"

        //             Map<String, String> IIS_CREDENTIALS = [
        //                 'webApplicationName': IIS_WEB_APPLICATION_NAME,
        //                 'appPath': IIS_APP_PATH,
        //                 'username': IIS_DEPLOY_CREDENTIALS_USR,
        //                 'password': IIS_DEPLOY_CREDENTIALS_PSW
        //             ]

        //             SendNotificationToDynatraceWin(['event': 'deploymentStarted'])

        //             ManageAppPoolWin(['deployServer': DEPLOY_SERVER, 'operation': 'Stop', 'appPoolName': APP_POOL_NAME])
        //             MsDeployArtifactWin(['deployServer': DEPLOY_SERVER, 'artifactZipFile': 'temp-artifact.zip', 'iisCredentials': IIS_CREDENTIALS])
        //             ManageAppPoolWin(['deployServer': DEPLOY_SERVER, 'operation': 'Start', 'appPoolName': APP_POOL_NAME])

        //             SendNotificationToDynatraceWin(['event': 'deploymentFinished'])
        //         }
        //     }
        // }
    }

    post {
        always {
            script {
                echo "Project pipeline end"
            }
        }
    }
}