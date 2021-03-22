﻿using EventArgsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace TrajectoryGeneratorNonHolonomeNS
namespace ClasseVector //ajouté
{
    public class TrajectoryGeneratorNonHolonome
    {
        int robotId;

        double samplingFreq;

        Location currentLocationRefTerrain;
        Location wayPointLocation;
        Location ghostLocationRefTerrain;

        double accelLineaire, accelAngulaire;
        double vitesseLineaireMax, vitesseAngulaireMax;

        AsservissementPID PID_Position_Lineaire;
        AsservissementPID PID_Position_Angulaire;

        public TrajectoryGeneratorNonHolonome(int id)
        {
            robotId = id;
            InitRobotPosition(0, 0, 0);
            InitPositionPID();

            //Initialisation des vitesse et accélérations souhaitées
            accelLineaire = 0.5; //en m.s-2
            accelAngulaire = 0.5 * Math.PI * 1.0; //en rad.s-2

            vitesseLineaireMax = 1; //en m.s-1               
            vitesseAngulaireMax = 1 * Math.PI * 1.0; //en rad.s-1
        }

        void InitPositionPID()
        {
            PID_Position_Lineaire = new AsservissementPID(20.0, 10.0, 0, 100, 100, 1);
            PID_Position_Angulaire = new AsservissementPID(20.0, 10.0, 0, 5 * Math.PI, 5 * Math.PI, Math.PI);
        }

        public void InitRobotPosition(double x, double y, double theta)
        {
            Location old_currectLocation = currentLocationRefTerrain;
            currentLocationRefTerrain = new Location(x, y, theta, 0, 0, 0);
            wayPointLocation = new Location(x, y, theta, 0, 0, 0);
            ghostLocationRefTerrain = new Location(x, y, theta, 0, 0, 0);
            PIDPositionReset();
        }

        public void OnPhysicalPositionReceived(object sender, LocationArgs e)
        {
            if (robotId == e.RobotId)
            {
                currentLocationRefTerrain = e.Location;
                CalculateGhostPosition();
                PIDPosition();
            }
        }



        double ptCibleprojete = 0;
        double xCp = 0;
        double yCp = 0;
        double vLinG = 0;
        double vLinGarret = 0;
        double dLinG = 0; //G de ghost
        double darret = 0;
        double thetaCible = 0;
        double thetaEcart = 0;


        void CalculateGhostPosition()
        {
            //A remplir
            
            //On renvoie la position du ghost pour affichage
            OnGhostLocation(robotId, ghostLocationRefTerrain);
        }


        void PIDPosition()
        {
            //A remplir
            //ON EN EST LA
            thetaCible = Math.Pow(Math.Atan((wayPointLocation.Y - currentLocationRefTerrain.Y) / (wayPointLocation.X - currentLocationRefTerrain.X)),2);
            thetaEcart = ghostLocationRefTerrain.Theta % wayPointLocation.Theta; //% = modulo ?!?!!!!!! a verifier !!!

            Vector vectorCible = new Vector(wayPointLocation.X, wayPointLocation.Y ;
            Vector vectorGhost = new Vector(ghostLocationRefTerrain.X, ghostLocationRefTerrain.Y ; //vecteur ghost
            //pt projeté cible = produit scalaire entre vecteur cible et vecteur ghost

            dLinG = Math.Sqrt(Math.Pow((xCp - ghostLocationRefTerrain.X), 2) + Math.Pow((yCp - ghostLocationRefTerrain.Y), 2)) ;
            vLinGarret = vLinG / 2 * dLinG;


            if(dLinG>darret)
            {
                vLinG += accelLineaire/50;
            }
            /////

            double vLineaireRobot=0, vAngulaireRobot=0;
            /////

            //Si tout c'est bien passé, on envoie les vitesses consigne.
            OnSpeedConsigneToRobot(robotId, (float)vLineaireRobot, (float)vAngulaireRobot);
        }

        void PIDPositionReset()
        {
            if (PID_Position_Angulaire != null && PID_Position_Lineaire != null)
            {
                PID_Position_Lineaire.ResetPID(0);
                PID_Position_Angulaire.ResetPID(0);
            }
        }

        /*************************************** Outgoing Events ************************************/

        public event EventHandler<LocationArgs> OnGhostLocationEvent;
        public virtual void OnGhostLocation(int id, Location loc)
        {
            var handler = OnGhostLocationEvent;
            if (handler != null)
            {
                handler(this, new LocationArgs { RobotId = id, Location = loc });
            }
        }

        public event EventHandler<PolarSpeedArgs> OnSpeedConsigneEvent;
        public virtual void OnSpeedConsigneToRobot(int id, float vLineaire, float vAngulaire)
        {
            var handler = OnSpeedConsigneEvent;
            if (handler != null)
            {
                handler(this, new PolarSpeedArgs { RobotId = id, Vx = vLineaire, Vy = 0, Vtheta = vAngulaire});
            }
        }
    }
}
