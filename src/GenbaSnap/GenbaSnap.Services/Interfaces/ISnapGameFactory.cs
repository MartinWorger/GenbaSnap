using GenbaSnap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GenbaSnap.Domain.Enums;

namespace GenbaSnap.Services.Interfaces
{
    public interface ISnapGameFactory
    {
        public SnapGame CreateSnapGame(GameType gameType, int noPlayers);
    }
}
