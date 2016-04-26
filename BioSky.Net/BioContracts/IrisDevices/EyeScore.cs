using BioService;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioContracts.IrisDevices
{
  public class Eyes
  {
    public Bitmap LeftEye;
    public Bitmap RightEye;
    public void Reset() { LeftEye = RightEye = null; }
  }

  public class EyeScore
  {
    public EyeScore(EyeType eyeType, int totalScore, int usableArea)
    {
      EyeType = eyeType;
      TotalScore = totalScore;
      UsableArea = usableArea;
    }
    public int TotalScore { get; private set; }
    public int UsableArea { get; private set; }
    public EyeType EyeType { get; private set; }
    bool Qualified { get { return TotalScore > 0 && UsableArea > 0; } }
  }
}
