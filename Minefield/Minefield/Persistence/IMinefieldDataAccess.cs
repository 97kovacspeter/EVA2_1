using System;
using System.Threading.Tasks;
namespace Minefield.Persistence
{
    public struct ReturnData
    {
        public int gameTime;
        public MinefieldTable table;
    }

    public interface IMinefieldDataAccess
    {
        /// <summary>
        /// Fájl betöltése.
        /// </summary>
        /// <param name="path">Elérési útvonal.</param>
        /// <returns>A fájlból beolvasott játéktábla.</returns>
        Task<ReturnData> LoadAsync(String path);

        /// <summary>
        /// Fájl mentése.
        /// </summary>
        /// <param name="path">Elérési útvonal.</param>
        /// <param name="table">A fájlba kiírandó játéktábla.</param>
        Task SaveAsync(String path, MinefieldTable table, int GameTime);
    }
}