using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans.KeyValueStore.Grains.Interfaces;
using Orleans.Runtime;

namespace Orleans.Grpc.Grains.Row
{
    public class RowDefinitionDefinitionGrain : Grain, IRowDefinitionGrain
    {
        private readonly IPersistentState<Dictionary<string, RowColumn>> columns;
        private readonly ILogger<RowDefinitionDefinitionGrain> logger;
        private readonly IPersistentState<RowState> rowState;

        public RowDefinitionDefinitionGrain(ILogger<RowDefinitionDefinitionGrain> logger,
            [PersistentState("columns", "rowStorage")]
            IPersistentState<Dictionary<string, RowColumn>> columns,
            [PersistentState("row", "rowStorage")] IPersistentState<RowState> rowState
        )
        {
            this.logger = logger;
            this.rowState = rowState;
            this.columns = columns;
        }

        /// <summary>
        /// Adds a column definition with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The new column name.</param>
        /// <returns>A task representing the result.</returns>
        /// <exception cref="Exception">Thrown when a duplicate column is attempted to be added.</exception>
        public async Task AddColumn(string name)
        {
            if (this.columns.State.ContainsKey(name))
            {
                throw new Exception($"Column with {name} already exists.");
            }

            this.columns.State.Add(name, new RowColumn());
            await this.columns.WriteStateAsync();
        }

        /// <summary>
        /// Adds a set of column definitions with the specified <paramref name="names"/>.
        /// </summary>
        /// <param name="names">A set of new column names to add.</param>
        /// <returns>A task representing the result.</returns>
        /// <exception cref="Exception">Thrown when a duplicate column is attempted to be added.</exception>
        public async Task AddColumns(params string[] names)
        {
            foreach (var name in names)
            {
                if (this.columns.State.ContainsKey(name))
                {
                    throw new Exception($"Column with {name} already exists.");
                }

                this.columns.State.Add(name, new RowColumn());
            }

            await this.columns.WriteStateAsync();
        }

        /// <summary>
        /// Gets a column definition with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The column definition</returns>
        /// <exception cref="Exception">Thrown when a column doesn't exist.</exception>
        public Task<RowColumn> GetColumn(string name)
        {
            if (!this.columns.State.ContainsKey(name))
            {
                throw new Exception($"Column with {name} doesn't exists");
            }

            return Task.FromResult(this.columns.State[name]);
        }
        
        /// <summary>
        /// Gets all defined columns.
        /// </summary>
        /// <returns>The list of columns definitions</returns>
        public Task<Dictionary<string, RowColumn>> Get() => Task.FromResult(columns.State);

        /// <summary>
        /// Removes a column definition with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The column name to delete.</param>
        /// <returns>A task representing the result.</returns>
        /// <exception cref="Exception">Thrown when a column is attempted to be removed and doesn't exist.</exception>
        public async Task RemoveColumn(string name)
        {
            if (!this.columns.State.ContainsKey(name))
            {
                throw new Exception($"Column with {name} doesn't exists.");
            }

            this.columns.State.Remove(name);
            await this.columns.WriteStateAsync();
        }
    }
}