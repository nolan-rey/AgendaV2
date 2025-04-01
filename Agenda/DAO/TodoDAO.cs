using System;
using System.Collections.Generic;
using System.Linq;
using Agenda.Models;

namespace Agenda.DAO
{
    public static class TodoDAO
    {
        /// <summary>
        /// Récupère toutes les tâches
        /// </summary>
        public static List<Todo> GetAll()
        {
            using var context = new AgendaContext();
            return context.Todos.ToList();
        }

        /// <summary>
        /// Récupère une tâche par son ID
        /// </summary>
        public static Todo? GetById(int id)
        {
            using var context = new AgendaContext();
            return context.Todos.FirstOrDefault(t => t.Id == id);
        }

        /// <summary>
        /// Ajoute une nouvelle tâche
        /// </summary>
        public static void Add(Todo todo)
        {
            using var context = new AgendaContext();
            context.Todos.Add(todo);
            context.SaveChanges();
        }

        /// <summary>
        /// Met à jour une tâche existante
        /// </summary>
        public static void Update(Todo updatedTodo)
        {
            using var context = new AgendaContext();
            context.Todos.Update(updatedTodo);
            context.SaveChanges();
        }

        /// <summary>
        /// Supprime une tâche par ID
        /// </summary>
        public static void Delete(int id)
        {
            using var context = new AgendaContext();
            var todo = context.Todos.Find(id);
            if (todo != null)
            {
                context.Todos.Remove(todo);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Recherche des tâches par mot-clé dans le titre
        /// </summary>
        public static List<Todo> Search(string keyword)
        {
            using var context = new AgendaContext();
            return context.Todos
                .Where(t => t.Title.Contains(keyword))
                .ToList();
        }

        /// <summary>
        /// Bascule l’état de complétion d’une tâche
        /// </summary>
        public static void ToggleCompleted(int id, bool isCompleted)
        {
            using var context = new AgendaContext();
            var todo = context.Todos.Find(id);
            if (todo != null)
            {
                todo.IsCompleted = isCompleted;
                context.SaveChanges();
            }
        }
    }
}
