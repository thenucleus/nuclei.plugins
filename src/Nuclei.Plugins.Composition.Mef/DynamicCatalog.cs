using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuclei.Plugins.Core;

namespace Nuclei.Plugins.Composition.Mef
{
    public sealed class DynamicCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {
        private readonly ISatisfyPluginRequests _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicCatalog"/> class.
        /// </summary>
        /// <param name="repository"></param>
        public DynamicCatalog(ISatisfyPluginRequests repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            _repository = repository;
        }

        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;

        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;

        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            return base.GetExports(definition);
        }

        public override IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
                return base.Parts;
            }
        }
    }
}
