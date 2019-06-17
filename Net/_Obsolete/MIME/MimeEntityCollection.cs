using System;
using System.Collections;
using System.Collections.Generic;

namespace LumiSoft.Net.Mime
{
    /// <summary>
    /// Mime entity collection.
    /// </summary>
    [Obsolete("See LumiSoft.Net.MIME or LumiSoft.Net.Mail namepaces for replacement.")]
    public class MimeEntityCollection : IEnumerable
    {
        private readonly List<MimeEntity> _entities;
        private readonly MimeEntity _ownerEntity;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="ownerEntity">Mime entity what owns this collection.</param>
        internal MimeEntityCollection(MimeEntity ownerEntity)
        {
            _ownerEntity = ownerEntity;

            _entities = new List<MimeEntity>();
        }

        /// <summary>
        /// Gets mime entities count in the collection.
        /// </summary>
        public int Count => _entities.Count;

        /// <summary>
        /// Gets mime entity at specified index.
        /// </summary>
        public MimeEntity this[int index] => (MimeEntity)_entities[index];

        /// <summary>
        /// Creates a new mime entity to the end of the collection.
        /// </summary>
        /// <returns></returns>
        public MimeEntity Add()
        {
            var entity = new MimeEntity();
            Add(entity);

            return entity;
        }

        /// <summary>
        /// Adds specified mime entity to the end of the collection.
        /// </summary>
        /// <param name="entity">Mime entity to add to the collection.</param>
        public void Add(MimeEntity entity)
        {
            // Allow to add only for multipart/xxx...
            if ((_ownerEntity.ContentType & MediaType_enum.Multipart) == 0)
            {
                throw new Exception("You don't have Content-Type: multipart/xxx. Only Content-Type: multipart/xxx can have nested mime entities !");
            }
            // Check boundary, this is required parameter for multipart/xx
            if (_ownerEntity.ContentType_Boundary == null || _ownerEntity.ContentType_Boundary.Length == 0)
            {
                throw new Exception("Please specify Boundary property first !");
            }

            _entities.Add(entity);
        }

        /// <summary>
        /// Clears the collection of all mome entities.
        /// </summary>
        public void Clear()
        {
            _entities.Clear();
        }

        /// <summary>
        /// Gets if collection contains specified mime entity.
        /// </summary>
        /// <param name="entity">Mime entity.</param>
        /// <returns></returns>
        public bool Contains(MimeEntity entity)
        {
            return _entities.Contains(entity);
        }

        /// <summary>
        /// Gets enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

        /// <summary>
        /// Inserts a new mime entity into the collection at the specified location.
        /// </summary>
        /// <param name="index">The location in the collection where you want to add the mime entity.</param>
        /// <param name="entity">Mime entity.</param>
        public void Insert(int index, MimeEntity entity)
        {
            // Allow to add only for multipart/xxx...
            if ((_ownerEntity.ContentType & MediaType_enum.Multipart) == 0)
            {
                throw new Exception("You don't have Content-Type: multipart/xxx. Only Content-Type: multipart/xxx can have nested mime entities !");
            }
            // Check boundary, this is required parameter for multipart/xx
            if (_ownerEntity.ContentType_Boundary == null || _ownerEntity.ContentType_Boundary.Length == 0)
            {
                throw new Exception("Please specify Boundary property first !");
            }

            _entities.Insert(index, entity);
        }

        /// <summary>
        /// Removes mime entity at the specified index from the collection.
        /// </summary>
        /// <param name="index">Index of mime entity to remove.</param>
        public void Remove(int index)
        {
            _entities.RemoveAt(index);
        }

        /// <summary>
        /// Removes specified mime entity from the collection.
        /// </summary>
        /// <param name="entity">Mime entity to remove.</param>
        public void Remove(MimeEntity entity)
        {
            _entities.Remove(entity);
        }
    }
}
