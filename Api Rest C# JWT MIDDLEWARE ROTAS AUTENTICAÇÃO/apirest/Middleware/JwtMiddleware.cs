public class JwtMiddleware {
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context) {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token != null) {
            try {
                // Valida e extrai as Claims (identidade)
                var userPrincipal = TokenService.ValidarToken(token);
                
                // IMPORTANTE: Isso faz o [Authorize] funcionar no C#
                context.User = userPrincipal;
                
                // Mantém o "modelo Node" que você queria também:
                context.Items["User"] = userPrincipal; 
            } catch {
                // Token inválido: não faz nada, o [Authorize] vai barrar no controller
            }
        }

        await _next(context);
    }
}