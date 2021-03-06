﻿from System.Collections.Generic import List

import clr
clr.AddReferenceToFileAndPath("MiRo.SimHexWorld.Engine.dll")
clr.AddReferenceToFileAndPath("TomShane.Neoforce.Controls.dll")
clr.AddReferenceToFileAndPath("Microsoft.Xna.Framework.dll")

from MiRo.SimHexWorld.Engine.Types import UnitAction
from MiRo.SimHexWorld.Engine.UI import MainWindow
from TomShane.Neoforce.Controls import ImageBox, MouseButton
from MiRo.SimHexWorld.Engine.UI.Controls import GameMapBox
from MiRo.SimHexWorld.Engine.UI import NotificationType
from Microsoft.Xna.Framework.Input import Keys

""" 
	MainWindow class handlers 
"""
class Window:
	"""
		init window
	"""
	def Initialize(self, parent):
		self.parent = parent

		""" init dialogs """
		self.parent.CreateWindow("Pedia", "Content//Controls//InfoPediaDialog", False)
		self.parent.CreateWindow("ScienceSelect", "Content//Controls//ScienceSelectDialog", False)
		self.parent.CreateWindow("MapOptions", "Content//Controls//MapOptionDialog", False)
		self.parent.CreateWindow("PolicyChoose", "Content//Controls//PolicyChooseDialog", False)
		self.parent.CreateWindow("Diplomacy", "Content//Controls//DiplomacyDialog", False)

	"""
		handle keys pressed
	"""
	def HandleKey( self, window, keyEventArgs ):
		""" Main View """
		if window.View == MainWindow.MapView.Main:
			if keyEventArgs.Key == Keys.Left:
				window.GetControl("MapBox").MoveCenter(-1,0)
				keyEventArgs.Handled = True

			if keyEventArgs.Key == Keys.Right:
				window.GetControl("MapBox").MoveCenter(1,0)
				keyEventArgs.Handled = True

			if keyEventArgs.Key == Keys.Up:
				window.GetControl("MapBox").MoveCenter(0,-1)
				keyEventArgs.Handled = True

			if keyEventArgs.Key == Keys.Down:
				window.GetControl("MapBox").MoveCenter(0,1)
				keyEventArgs.Handled = True

		""" City View """
		if window.View == MainWindow.MapView.City:
			if keyEventArgs.Key == Keys.Left:
				window.FocusPreviousCity()
				keyEventArgs.Handled = True

			if keyEventArgs.Key == Keys.Right:
				window.FocusNextCity()
				keyEventArgs.Handled = True

	"""
		focus changed handler
	"""
	def FocusChanged(self, window, mapChangeArgs):
		if (mapChangeArgs.Map == None):
			return

		self.parent.Focus = mapChangeArgs.UpdatedTiles[0]

	"""
		update function / called every 3 seconds
	"""
	def Update(self, window, gametime ):
		pass

	"""
		update control values
	"""
	def ShowMainControls(self, show):
		""" overview controls """
		self.ShowIfExists("OverviewTop", show )
		self.ShowIfExists("OverviewBottomRight", show )
		self.ShowIfExists("OverviewMap", show )
		self.ShowIfExists("MapOptionsToggle", show )
		self.ShowIfExists("NoteOptions", show )

		""" science controls """
		self.ShowIfExists("LeftTopCorner", show )
		self.ShowIfExists("ResearchProgress", show )
		self.ShowIfExists("ScienceDetail", show )

		""" units controls """
		self.ShowIfExists("UnitDetail", show )
		self.ShowIfExists("UnitAction0", show )
		self.ShowIfExists("UnitAction1", show )

		""" right top controls """
		self.ShowIfExists("RightTopBg", show )
		self.ShowIfExists("BtnPolicies", show )
		self.ShowIfExists("BtnDiplomacy", show )
		self.ShowIfExists("BtnAdvisors", show )

		""" notifcations """
		self.ShowIfExists("Notification0", show )
		self.ShowIfExists("Notification1", show )
		self.ShowIfExists("Notification2", show )
		self.ShowIfExists("Notification3", show )
		self.ShowIfExists("Notification4", show )
		self.ShowIfExists("Notification5", show )
		self.ShowIfExists("Notification6", show )
		self.ShowIfExists("Notification7", show )
		self.ShowIfExists("Notification8", show )
		self.ShowIfExists("Notification9", show )

	def ShowCityControls(self, show):
		""" background """
		self.ShowIfExists("LeftSide", show )
		self.ShowIfExists("RightSide", show )

		""" city controls """
		self.ShowIfExists("CitySidebar", show )
		self.ShowIfExists("CityExit", show )
		self.ShowIfExists("CityCitizen", show )
		self.ShowIfExists("BuildingsToggle", show )
		self.ShowIfExists("BuildingsList", show )

		""" production """
		self.ShowIfExists("CurrentBuilding", show )
		self.ShowIfExists("CurrentProductionMeter", show )

		#handler.Callback("show city: " + str(show),None)

	def ShowIfExists(self, name, show):
		control = self.parent.GetControl(name)

		if not(control is None):
			control.Visible = show

	def ToggleView(self, window):
		if window.View == MainWindow.MapView.Main:
			""" switch from main to city view """
			self.ShowMainControls(False)
			self.ShowCityControls(True)

			self.oldZoomState = window.MapBox.Zoom
			window.MapBox.Zoom = "Near"

			window.CurrentCity.InDetailView = True
			window.View = MainWindow.MapView.City
			
		elif window.View == MainWindow.MapView.City:
			""" switch from city to main view """
			self.ShowMainControls(True)
			self.ShowCityControls(False)

			window.MapBox.Zoom = self.oldZoomState

			window.View = MainWindow.MapView.Main
			window.CurrentCity.InDetailView = False
			window.CurrentCity = None
	
	def CityOpened(self, window, city):
		window.CurrentCity = city;
            
		if window.View == MainWindow.MapView.Main:
			self.ToggleView(window)

		self.UpdateCityControls(window)

	def UpdateCityControls(self, window):
		if window.CurrentCity == None:
			return

		window.GetControl("CityCitizen").Text = "Citizen: " + str(window.CurrentCity.Citizen) + " (" + str(window.CurrentCity.Population) + ")"
		
		if window.CurrentCity.CurrentBuildingTarget != None:
			window.GetControl("CurrentBuilding").Text = window.CurrentCity.CurrentBuildingTarget.Title + "(" + str((int)(window.CurrentCity.ProductionReady * 100)) + "%)"
		else:
			window.GetControl("CurrentBuilding").Text = ""

		window.GetControl("BuildingsList").Items.Clear()

		for building in window.CurrentCity.Buildings:
			window.GetControl("BuildingsList").Items.Add(building)

	def CitySelected(self, window, city):
		pass

	def UnitUnselected(self, window):
		window.CurrentUnit = None

		window.GetControl("UnitAction0").Visible = False
		window.GetControl("UnitAction1").Visible = False
		window.GetControl("UnitAction2").Visible = False

	def HumanUnitSelected( self, window, unit):
		window.CurrentUnit = unit
		window.CurrentUnitActions = List[UnitAction]()

		i = 0
		for action in unit.Actions:
			window.CurrentUnitActions.Add(action)
		
			actionButton = window.GetControl("UnitAction" + str(i))
			
			if actionButton != None:
				actionButton.Visible = True

			i = i + 1

		return

	def ActionButton_Click( self, window, sender, args ):
		if window.CurrentUnitActions == None:
			return
		
		num = int(sender.Name[-1])	
		action = window.CurrentUnitActions[num]

		if window.CurrentUnit != None:
			window.CurrentUnit.Execute(action)
			args.Handled = True

	"""
		open pedia after unit icon clicked
	"""
	def Unit_Click( self, window, sender, args ):
		if self.parent.CurrentUnit != None:
			window.GetWindow("Pedia").Invoke( "Focus", self.parent.CurrentUnit.Data )
			window.GetWindow("Pedia").ShowModal()

	def BtnAdvisors_Click( self, window, sender, args ):
		pass

	""" 
		toggle map option view 
	"""
	def MapOptions_Click( self, window, sender, args ):
		window.GetWindow("MapOptions").Visible = not window.GetWindow("MapOptions").Visible;

	"""
		open pedia after research icon clicked
	"""
	def Research_Click( self, window, sender, args ):
		if game.Human.CurrentResearch != None:
			window.GetWindow("Pedia").Invoke( "Focus",  game.Human.CurrentResearch )
			window.GetWindow("Pedia").ShowModal()

	def Science_Click( self, window, sender, args ):
		window.GetWindow("ScienceSelect").ShowModal()

	def BtnPolicies_Click( self, window, sender, args ):
		window.GetWindow("PolicyChoose").ShowModal()

	def BtnDiplomacy_Click( self, window, sender, args ):
		window.GetWindow("Diplomacy").ShowModal()

	def BuildingsList_ItemIndexChanged( self, window, sender, args ):
		
		if sender.ItemIndex != -1:
			for item in self.parent.GetControl("BuildingsList").ContextMenu.Items:
				item.Enabled = True

	def BuildingDelete_Click( self, window, sender, args ):
		handler.Callback("delete", None)

	def CityExit_Click( self, window, sender, args ):
		self.ToggleView(window)

	def BuildingsToggle_Click( self, window, sender, args ):
		self.parent.GetControl("BuildingsList").Visible = not self.parent.GetControl("BuildingsList").Visible

	"""
		notification clicked
	"""
	def Notification_Click( self, window, sender, args ):
		num = int(sender.Name[-1])	
		
		if window.Messages.Count > num:

			if args.Button == MouseButton.Left:

				if window.Messages[num].Type == NotificationType.CityGrowth or window.Messages[num].Type == NotificationType.CityDecline or window.Messages[num].Type == NotificationType.FoundCity:
					window.MapBox.CenterAt(window.Messages[num].Obj.Point)

				elif window.Messages[num].Type == NotificationType.ImprovementReady:
					window.MapBox.CenterAt(window.Messages[num].Obj[1])
 
				elif window.Messages[num].Type == NotificationType.Science:
					window.GetWindow("ScienceSelect").ShowModal()

				elif window.Messages[num].Type == NotificationType.PolicyReady:
					window.GetWindow("PolicyChoose").ShowModal()

				elif window.Messages[num].Type == NotificationType.ProducationReady:
					window.MapBox.CenterAt(window.Messages[num].Obj.Point)
					CurrentCity = window.Messages[num].Obj

			""" event is handled """
			window.Messages[num].Obsolete = True


window = Window()