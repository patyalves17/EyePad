using System;
using System.Collections.Generic;
using System.Linq;
using EyePad.Data;
using SQLite;
using Xamarin.Forms;

namespace EyePad.Model
{
    public class Note
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Date { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }

    }

    public class NoteRepository
    {
        private NoteRepository() { }

        private static SQLiteConnection database;
        private static readonly NoteRepository instance = new NoteRepository();
        static object locker = new object();
        public static NoteRepository Instance
        {
            get
            {
                if (database == null)
                {
                    database = DependencyService.Get<ISQLite>().GetConexao();
                    database.CreateTable<Note>();
                }
                return instance;
            }
        }


        public static int SaveNote(Note note)
        {
            lock (locker)
            {
                if (note.Id != 0)
                {
                    database.Update(note);
                    return note.Id;
                }
                else return database.Insert(note);
            }
        }

        public static IEnumerable<Note> GetNotes()
        {
            lock (locker)
            {
                return (from c in database.Table<Note>() select c).ToList();
            }
        }

        public static Note GetNote(int Id)
        {
            lock (locker)
            {
                return database.Table<Note>().Where(c => c.Id == Id).FirstOrDefault();
            }
        }

        public static int RemoverNote(int Id)
        {
            lock (locker)
            {
                return database.Delete<Note>(Id);
            }
        }
    }
}