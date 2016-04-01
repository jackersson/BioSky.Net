﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BioService;

namespace BioModule.Utils
{
  public interface IUserBioItemsUpdatable
  {
    void UpdateBioItemsController(IUserBioItemsController controller);
    void UpdateFromPhoto(Photo photo);
    void Clear();

    Photo CurrentPhoto { get; }
  }
}
