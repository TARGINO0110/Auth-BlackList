# Auth-BlackList

<h3>Execulte o comando abaixo na raiz do projeto redis-compose.yaml</h3>

```
docker-compose -f .\redis-compose.yaml up -d
```

<h3>O arquivo .yaml contem a seguinte configuração para uso do Redis e Redis Insight</h3>

```
version: "3"
services:
  redis:
    image: redis:7.2.5
    container_name: redis
    restart: always
    environment:
      - REDIS_PASSWORD=admin123
      - REDIS_REPLICATION_MODE=master
    ports:
      - 6379:6379
    volumes:
      - redis_volume_data:/data
  redisinsight:
    image: redislabs/redisinsight:1.14.0
    container_name: redisinsight
    restart: always
    environment:
      - REDISINSIGHT_REDIS_URL=redis://default:admin123@redis:6379
    ports:
      - 8001:8001
    volumes:
      - redis_insight_volume_data:/db
volumes: 
  redis_volume_data:
  redis_insight_volume_data:
```

<h3>Após execultar o comando do compose verifique a url http://localhost:8001 </h3>
