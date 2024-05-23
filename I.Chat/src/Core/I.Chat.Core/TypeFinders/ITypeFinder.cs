using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace I.Chat.Core.TypeFinders
{
    public interface ITypeFinder
    {
        IList<Assembly> GetAssemblies();

        IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true);

        IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);

        IEnumerable<Type> FindClassesOfType<TEntity>(bool onlyConcreteClasses = true);

        IEnumerable<Type> FindClassesOfType<TEntity>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true);
    }
}
