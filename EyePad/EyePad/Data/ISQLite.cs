using System;
using SQLite;

namespace EyePad.Data
{
    public interface ISQLite
    {
        SQLiteConnection GetConexao();
    }
}
