using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using QuantumConcepts.Common.Extensions;

namespace QuantumConcepts.Common.DataObjects {
    /// <summary>This exception stores information about conflicts that may prevent an object from being deleted.</summary>
    public class DeleteConflictException : Exception, IEnumerable<DeleteConflict> {
        /// <summary>The name of the object being deleted.</summary>
        /// <remarks>This is the friendly name, not the name of the type.</remarks>
        public string Name { get; set; }

        /// <summary>The plural name of the object being deleted.</summary>
        /// <remarks>This is the friendly name, not the name of the type.</remarks>
        public string PluralName { get; set; }

        /// <summary>The objec that is being deleted.</summary>
        public object DeletingObject { get; set; }

        /// <summary>A list of conflicts which may prevent the object from being deleted.</summary>
        public List<DeleteConflict> DeleteConflicts { get; set; }

        /// <summary>Creates a new, empty instance.</summary>
        public DeleteConflictException() {
            this.DeleteConflicts = new List<DeleteConflict>();
        }

        /// <summary>Creates a new instance using the provided parameters.</summary>
        /// <param name="name"><see cref="Name"/></param>
        /// <param name="pluralName"><see cref="PluralName"/></param>
        /// <param name="deletingObject"><see cref="DeletingObject"/></param>
        public DeleteConflictException(string name, string pluralName, object deletingObject)
            : this() {
            this.Name = name;
            this.PluralName = pluralName;
            this.DeletingObject = deletingObject;
        }

        /// <summary>Adds or updates a <see cref="DeleteConflict"/> with the provided parameters.</summary>
        /// <param name="name"><see cref="DeleteConflict.Name"/></param>
        /// <param name="pluralName"><see cref="DeleteConflict.PluralName"/></param>
        /// <param name="objects"><see cref="DeleteConflict.Objects"/></param>
        public DeleteConflict Add(Type childType, string name, string pluralName, IEnumerable<object> objects) {
            DeleteConflict mergedConflict = MergeConflict(childType, name, pluralName, objects);

            this.DeleteConflicts.Add(mergedConflict);

            return Add(mergedConflict);
        }

        /// <summary>Adds or updates a <see cref="DeleteConflict"/>.</summary>
        /// <param name="conflict">The conflict to add or update.</param>
        /// <returns>The provided conflict or the existing conflict into which the provided conflict was merged.</returns>
        public DeleteConflict Add(DeleteConflict conflict) {
            DeleteConflict mergedConflict = MergeConflict(conflict);

            this.DeleteConflicts.Add(conflict);

            return conflict;
        }

        /// <summary>Adds or updates a range of <see cref="DeleteConflict"/>, see <see cref="Add(DeleteConflict)"/></summary>
        /// <param name="conflicts">The conflicts to add or update.</param>
        public void AddRange(IEnumerable<DeleteConflict> conflicts) {
            conflicts?.ForEach(o => Add(o));
        }

        private DeleteConflict MergeConflict(DeleteConflict conflictToMerge) {
            DeleteConflict existingConflict = this.DeleteConflicts.SingleOrDefault(o => string.Equals(o.Name, conflictToMerge.Name) && string.Equals(o.PluralName, conflictToMerge.PluralName));

            if (existingConflict != null) {
                existingConflict.Objects = existingConflict.Objects.Union(conflictToMerge.Objects);

                return existingConflict;
            }
            else
                return conflictToMerge;
        }

        private DeleteConflict MergeConflict(Type childType, string name, string pluralName, IEnumerable<object> objects) {
            DeleteConflict conflict = this.DeleteConflicts.SingleOrDefault(o => object.Equals(o.ChildType, childType) && string.Equals(o.Name, name) && string.Equals(o.PluralName, pluralName));

            if (conflict != null)
                conflict.Objects = conflict.Objects.Union(objects);
            else
                conflict = new DeleteConflict(childType, name, pluralName, objects);

            return conflict;
        }

        /// <summary>Throws this exception instance if it contains any <see cref="DeleteConflict">conflicts</see>.</summary>
        public void ThrowIfNotEmpty() {
            //Throw if there are conflicts and any of those conflicts have at least one conflicting object.
            if (!this.IsNullOrEmpty() && this.Any(o => !o.IsNullOrEmpty()))
                throw this;
        }

        public override string Message { get { return GetMessage(); } }

        public string GetMessage(string separator = "\r\n") {
            if (!this.DeleteConflicts.IsNullOrEmpty())
                return string.Join(separator, this.DeleteConflicts.Select(dc => dc.GetMessage()).ToArray());
            else
                return null;
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        public IEnumerator<DeleteConflict> GetEnumerator() {
            return this.DeleteConflicts.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        IEnumerator IEnumerable.GetEnumerator() {
            return this.DeleteConflicts.GetEnumerator();
        }

        public override string ToString() {
            return GetMessage();
        }
    }

    /// <summary>Stores information regarding a conflict which may prevent an object from being deleted.</summary>
    public class DeleteConflict : IEnumerable<object> {
        /// <summary>The type of the child object.</summary>
        public Type ChildType { get; set; }

        /// <summary>The name of the object being deleted.</summary>
        /// <remarks>This is the friendly name, not the name of the type.</remarks>
        public string Name { get; set; }

        /// <summary>The plural name of the object being deleted.</summary>
        /// <remarks>This is the friendly name, not the name of the type.</remarks>
        public string PluralName { get; set; }

        /// <summary>The objects which may prevent the related <see cref="DeleteConflictException.DeletingObject">deleting object</see> from being deleted.</summary>
        public IEnumerable<object> Objects { get; set; }

        /// <summary>Creates a new, empty instance.</summary>
        public DeleteConflict() {
            this.Objects = new List<object>();
        }

        /// <summary>Creates a new instance using the provided parameters.</summary>
        /// <param name="childType"><see cref="ChildType"/></param>
        /// <param name="name"><see cref="Name"/></param>
        /// <param name="pluralName"><see cref="PluralName"/></param>
        /// <param name="objects"><see cref="Objects"/></param>
        public DeleteConflict(Type childType, string name, string pluralName, IEnumerable<object> objects)
            : this() {
            this.ChildType = childType;
            this.Name = name;
            this.PluralName = pluralName;
            this.Objects = objects;
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        public IEnumerator<object> GetEnumerator() {
            return this.Objects.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        IEnumerator IEnumerable.GetEnumerator() {
            return this.Objects.GetEnumerator();
        }

        public string GetHeading(int showingCount = 3) {
            int count = this.Objects.Count();

            return $"{count} related {(count == 1 ? this.Name : this.PluralName)}{(count > showingCount ? $" (showing first {showingCount})" : null)}";
        }

        public string GetMessage(int objectLimit = 3) {
            StringBuilder message = new StringBuilder();

            message.Append(GetHeading());
            message.Append(": ");
            message.Append(string.Join(", ", this.Objects.Take(objectLimit).Select(o => o.ToString()).ToArray()));

            return message.ToString();
        }

        public override string ToString() {
            return GetMessage();
        }
    }
}
