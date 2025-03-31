using System.Collections.Generic;
using System.Linq;
using Agenda.Models;

namespace Agenda.DAO
{
    public static class ContactDAO
    {
        /// <summary>
        /// Récupère tous les contacts de la base
        /// </summary>
        public static List<Contact> GetAll()
        {
            using var context = new AgendaContext();
            return context.Contacts.ToList();
        }

        /// <summary>
        /// Récupère un contact par son ID
        /// </summary>
        public static Contact? GetById(int id)
        {
            using var context = new AgendaContext();
            return context.Contacts.FirstOrDefault(c => c.Id == id);
        }

        /// <summary>
        /// Recherche un contact par nom ou prénom (partiel)
        /// </summary>
        public static List<Contact> Search(string keyword)
        {
            using var context = new AgendaContext();
            return context.Contacts
                .Where(c => c.FirstName.Contains(keyword) || c.LastName.Contains(keyword))
                .ToList();
        }

        /// <summary>
        /// Ajoute un nouveau contact
        /// </summary>
        public static void Add(Contact contact)
        {
            using var context = new AgendaContext();
            context.Contacts.Add(contact);
            context.SaveChanges();
        }

        /// <summary>
        /// Met à jour un contact existant
        /// </summary>
        public static void Update(Contact updatedContact)
        {
            using var context = new AgendaContext();
            context.Contacts.Update(updatedContact);
            context.SaveChanges();
        }

        /// <summary>
        /// Supprime un contact
        /// </summary>
        public static void Delete(int contactId)
        {
            using var context = new AgendaContext();
            var contact = context.Contacts.Find(contactId);
            if (contact != null)
            {
                context.Contacts.Remove(contact);
                context.SaveChanges();
            }
        }
    }
}
