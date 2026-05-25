namespace SMART.ERP.Application.Repository
{
    /// <summary>
    /// Ejecuta una operación dentro de una transacción de base de datos atómica.
    /// Necesario porque el repositorio (Ardalis) hace SaveChanges en cada operación;
    /// envolverlas en una transacción garantiza que confirmar/cancelar movimientos de
    /// inventario sea todo-o-nada.
    /// </summary>
    public interface IUnitOfWork
    {
        Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default);
    }
}
