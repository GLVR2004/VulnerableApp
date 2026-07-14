#!/bin/bash

BASE_URL="http://localhost:5018"

echo "Iniciando generación de tráfico y logs para VulnerableApp en $BASE_URL..."
echo "------------------------------------------------------------------------"

echo "[1/7] Ejecutando navegación general (30 visitas a inicio y otros controladores)..."
for i in {1..30}; do
  curl -s -o /dev/null "$BASE_URL/"
done

curl -s -o /dev/null "$BASE_URL/Home/Privacy"
curl -s -o /dev/null "$BASE_URL/Comment/Index"
curl -s -o /dev/null "$BASE_URL/Search/Index"
curl -s -o /dev/null "$BASE_URL/Auth/Login"

echo "[2/7] Ejecutando pruebas de búsqueda..."
for i in {1..100}; do
  curl -s -o /dev/null "$BASE_URL/Search/Index?search=usuario$i"
done

for i in {1..20}; do
  curl -s -o /dev/null "$BASE_URL/Search/Index?search="
done

for i in {1..20}; do
  curl -s -o /dev/null "$BASE_URL/Search/Index?search=%24%25%26%2A"
done

for i in {1..20}; do
  curl -s -o /dev/null "$BASE_URL/Search/Index?search=1%27%20OR%20%271%27%3D%271"
done

echo "[3/7] Ejecutando pruebas de autenticación..."
for i in {1..50}; do
  curl -s -o /dev/null -X POST "$BASE_URL/Auth/Login" \
       -d "username=admin&P_key=123"
done

for i in {1..100}; do
  curl -s -o /dev/null -X POST "$BASE_URL/Auth/Login" \
       -d "username=admin&P_key=clave_falsa_$i"
done

for i in {1..20}; do
  curl -s -o /dev/null -X POST "$BASE_URL/Auth/Login" \
       -d "username=usuario_fantasma_$i&P_key=12345"
done

echo "[4/7] Ejecutando pruebas de comentarios..."
for i in {1..100}; do
  curl -s -o /dev/null -X POST "$BASE_URL/Comment/AddComment" \
       -d "comment=Este es un comentario de prueba $i"
done

for i in {1..30}; do
  curl -s -o /dev/null -X POST "$BASE_URL/Comment/AddComment" \
       -d "comment=%3Cscript%3Ealert%281%29%3C%2Fscript%3E"
done

echo "[5/7] Ejecutando pruebas de consumo de API..."
for i in {1..200}; do
  curl -s -o /dev/null "$BASE_URL/api/user/1"
done

for i in {1..30}; do
  curl -s -o /dev/null "$BASE_URL/api/recurso_inexistente_$i"
done

for i in {1..30}; do
  curl -s -o /dev/null "$BASE_URL/api/user/99999$i"
done

# 6. Warnings
echo "[6/7] Generando eventos Warning..."
for i in {1..20}; do
  curl -s -o /dev/null -X POST "$BASE_URL/Comment/AddComment" \
       -d "comment="
done

# 7. Excepciones
echo "[7/7] Provocando excepciones..."
for i in {1..20}; do
  curl -s -o /dev/null "$BASE_URL/Home/Error"
done

echo "------------------------------------------------------------------------"
echo "Pruebas finalizadas. Puedes revisar Seq en http://localhost:5341 para validar tus registros."
