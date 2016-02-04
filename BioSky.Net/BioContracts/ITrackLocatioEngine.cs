﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts
{
  public interface ITrackLocationEngine
  {
    void Init();

    AsyncObservableCollection<TrackLocation> TrackLocations
    { get; }
  }
}
