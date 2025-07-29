<!DOCTYPE html>
<html lang="pt-BR">
<head><meta charset="UTF-8"><title>Produtos</title></head>
<body>
  <h2>Bem-vindo, {{USER}}!</h2>
  <h3>Produtos disponíveis:</h3>
  <ul>
    <li>Produto A</li>
    <li>Produto B</li>
    <li>Produto C</li>
  </ul>
  <form action="/logout" method="POST">
    <button type="submit">Logout</button>
  </form>
</body>
</html>
