using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceValidity
{
    class FaceDetect
    {
        public string faceId { get; set; }
    }
    class FaceVerify
    {
        public bool isIdentical { get; set; }
        public float confidence { get; set; }
    }
}
