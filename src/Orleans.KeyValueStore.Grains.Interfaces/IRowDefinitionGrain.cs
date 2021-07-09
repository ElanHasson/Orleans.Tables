using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orleans.KeyValueStore.Grains.Interfaces
{
    public interface IRowDefinitionGrain : IGrainWithStringKey
    {
        /// <summary>
        /// Adds a column definition with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The new column name.</param>
        /// <returns>A task representing the result.</returns>
        /// <exception cref="Exception">Thrown when a duplicate column is attempted to be added.</exception>
        Task AddColumn(string name);

        /// <summary>
        /// Adds a set of column definitions with the specified <paramref name="names"/>.
        /// </summary>
        /// <param name="names">A set of new column names to add.</param>
        /// <returns>A task representing the result.</returns>
        /// <exception cref="Exception">Thrown when a duplicate column is attempted to be added.</exception>
        Task AddColumns(params string[] names);

        /// <summary>
        /// Gets a column definition with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The column definition</returns>
        /// <exception cref="Exception">Thrown when a column doesn't exist.</exception>
        Task<RowColumn> GetColumn(string name);

        /// <summary>
        /// Gets all defined columns.
        /// </summary>
        /// <returns>The list of columns definitions</returns>
        Task<Dictionary<string, RowColumn>> Get();

        /// <summary>
        /// Removes a column definition with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The column name to delete.</param>
        /// <returns>A task representing the result.</returns>
        /// <exception cref="Exception">Thrown when a column is attempted to be removed and doesn't exist.</exception>
        Task RemoveColumn(string name);
    }
}