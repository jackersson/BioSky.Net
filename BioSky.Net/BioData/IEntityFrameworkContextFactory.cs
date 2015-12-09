using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioData
{
  public interface IEntityFrameworkContextFactory
  {
    T Create<T>() where T : DbContext;

    void Release<T>(T context) where T : DbContext;

  }
}
