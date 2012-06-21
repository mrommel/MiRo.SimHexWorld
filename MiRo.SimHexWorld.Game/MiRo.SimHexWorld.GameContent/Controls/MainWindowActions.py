from System.Collections.Generic import List

import clr
clr.AddReferenceToFileAndPath("MiRo.SimHexWorld.Engine.dll")
clr.AddReferenceToFileAndPath("TomShane.Neoforce.Controls.dll")

from MiRo.SimHexWorld.Engine.Types import UnitAction
from MiRo.SimHexWorld.Engine.UI import MainWindow
from TomShane.Neoforce.Controls import ImageBox
from MiRo.SimHexWorld.Engine.UI.Controls import GameMapBox

class Window:
	""" MainWindow class handlers """
	def Initialize(self, parent):
		self.parent = parent

	def FocusChanged(self, window, mapChangeArgs):
		if (mapChangeArgs.Map == None):
			return
		self.parent.Focus = mapChangeArgs.UpdatedTiles[0]
		return

	def ShowMainControls(self, show):
		""" overview controls """
		self.parent.GetControl("OverviewTop").Visible = show
		self.parent.GetControl("OverviewBottomRight").Visible = show
		self.parent.GetControl("OverviewMap").Visible = show
		self.parent.GetControl("MapOptions").Visible = show
		self.parent.GetControl("NoteOptions").Visible = show

		""" science controls """
		self.parent.GetControl("LeftTopCorner").Visible = show
		self.parent.GetControl("ResearchProgress").Visible = show
		self.parent.GetControl("ScienceDetail").Visible = show

		""" units controls """
		self.parent.GetControl("UnitDetail").Visible = show
		self.parent.GetControl("UnitAction0").Visible = show
		self.parent.GetControl("UnitAction1").Visible = show

		""" right top controls """
		self.parent.GetControl("RightTopBg").Visible = show
		self.parent.GetControl("BtnPolicies").Visible = show
		self.parent.GetControl("BtnDiplomacy").Visible = show
		self.parent.GetControl("BtnAdvisors").Visible = show

		""" notifcations """
		self.parent.GetControl("Notification0").Visible = show
		self.parent.GetControl("Notification1").Visible = show
		self.parent.GetControl("Notification2").Visible = show
		self.parent.GetControl("Notification3").Visible = show
		self.parent.GetControl("Notification4").Visible = show
		self.parent.GetControl("Notification5").Visible = show
		self.parent.GetControl("Notification6").Visible = show
		self.parent.GetControl("Notification7").Visible = show
		self.parent.GetControl("Notification8").Visible = show
		self.parent.GetControl("Notification9").Visible = show

	def ShowCityControls(self, show):
		""" background """
		self.parent.GetControl("LeftSide").Visible = show
		self.parent.GetControl("RightSide").Visible = show

		""" city controls """
		self.parent.GetControl("CitySidebar").Visible = show
		self.parent.GetControl("CityExit").Visible = show
		self.parent.GetControl("CityCitizen").Visible = show
		self.parent.GetControl("BuildingsToggle").Visible = show
		self.parent.GetControl("BuildingsList").Visible = show

		""" production """
		self.parent.GetControl("CurrentBuilding").Visible = show
		self.parent.GetControl("CurrentProductionMeter").Visible = show

		#handler.Callback("show city: " + str(show),None)

	def ToggleView(self, window):
		if window.View == MainWindow.MapView.Main:
			""" switch from main to city view """
			self.ShowMainControls(False)
			self.ShowCityControls(True)

			self.oldZoomState = window.MapBox.Zoom
			window.MapBox.Zoom = GameMapBox.ZoomState.VeryNear

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

	def Unit_Click( self, window, sender, args ):
		pass

	def BtnAdvisors_Click( self, window, sender, args ):
		pass

	def MapOptions_Click( self, window, sender, args ):
		""" toggle map option view """
		window.MapOptions.Visible = not window.MapOptions.Visible;

	def LblResearch_Click( self, window, sender, args ):
		""" if( Game.Human.CurrentResearch != None )
			ScienceInfoDialog.Show(Manager, Game.Human.CurrentResearch, "Science");"""
		pass

	def Science_Click( self, window, sender, args ):
		window.ScienceDialog.ShowModal()

	def BtnPolicies_Click( self, window, sender, args ):
		window.PolicyChooseDialog.ShowModal()

	def BtnDiplomacy_Click( self, window, sender, args ):
		window.DiplomacyDialog.ShowModal()

	def BuildingsList_ItemIndexChanged( self, window, sender, args ):
		
		if sender.ItemIndex != -1:
			for item in self.parent.GetControl("BuildingsList").ContextMenu.Items:
				item.Enabled = True
				#handler.Callback(str(item.Text) + " " + str(item.Enabled), None)

			#handler.Callback("enabled", None)
		#else:
			#for item in sender.ContextMenu.Items:
			#	item.Enabled = False

	def BuildingDelete_Click( self, window, sender, args ):
		handler.Callback("delete", None)

	def CityExit_Click( self, window, sender, args ):
		self.ToggleView(window)

	def BuildingsToggle_Click( self, window, sender, args ):
		self.parent.GetControl("BuildingsList").Visible = not self.parent.GetControl("BuildingsList").Visible
		#handler.Callback("toggle", None)

window = Window()