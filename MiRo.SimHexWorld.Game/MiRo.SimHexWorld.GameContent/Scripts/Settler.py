import clr
clr.AddReference('MiRo.SimHexWorld.Engine')

import MiRo.SimHexWorld.Engine

class SettlerScript:

    def __init__(self):
        self.unit = None
    
    def Init(self, unit):
        self.unit = unit
    
        handler.Callback('Inited: %s, %s, %s' % (self.unit.Point, self.unit.Player, self.unit.Data), self.unit)
   
    def Iterate(self, unit):
        
        
        
        handler.Callback('Iterated', self.unit)
    
script = SettlerScript()