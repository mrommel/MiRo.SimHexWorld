import clr
clr.AddReferenceToFileAndPath("MiRo.SimHexWorld.Engine.dll")
clr.AddReferenceToFileAndPath("TomShane.Neoforce.Controls.dll")

from MiRo.SimHexWorld.Engine.UI.Dialogs import MessageBox
from MiRo.SimHexWorld.Engine.Misc import Provider

""" 
	PolicyChooseWindow class handlers 
"""
class Window:
	
	"""
		init window
	"""
	def Initialize(self, parent):
		self.parent = parent

		self.UpdateVisibility()

	"""
		click callback for policy type adopt buttons for all policy types
	"""
	def Adopt_Click( self, window, sender, args ):
		name = sender.Name.Replace("Adopt", "").Replace("Button","")

		if not game.Human.AdoptPolicyType(Provider.GetPolicyType(name)):
			MessageBox.Show(window.Manager, "It was not possible to adopt " + name + ", because there is too little culture.\n\n" + str(game.Human.Culture) + " of culture present, but " + str(game.Human.CultureNeededForChange) + " needed.", "Adopt " + name)

		self.UpdateVisibility()
		window.Close()

	"""
		click callback for policy select buttons for all policies
	"""
	def Choose_Click( self, window, sender, args ):

		name = sender.Name.Replace("Button", "")

		if not game.Human.AdoptPolicy(Provider.GetPolicy(name)):
			MessageBox.Show(window.Manager, "It was not possible to adopt " + name + ", because there were to little culture.\n\n" + str(game.Human.Culture) + " of culture present, but " + str(game.Human.CultureNeededForChange) + " needed.", "Adopt " + name)

		self.UpdateVisibility()
		window.Close()

	"""
		click callback for close button
	"""
	def Close_Click( self, window, sender, args ):
		window.Close()

	def HasAdopted( self, policyTypeName ):
		return game.Human.AdoptedPolicyTypes.Contains(Provider.GetPolicyType(policyTypeName))

	def CanAdopt( self, policyTypeName ):
		return game.Human.HasEra( Provider.GetPolicyType(policyTypeName).EraName )

	"""
		updates the visibility of the adopt/choose buttons of the policy types and policies
	"""
	def UpdateVisibility( self ):

		""" Tradition """
		hasAdoptedTradition = self.HasAdopted("Tradition")
		self.parent.GetControl("AdoptTraditionButton").Visible = not hasAdoptedTradition
		self.parent.GetControl("AristocracyButton").Visible = hasAdoptedTradition
		self.parent.GetControl("LandedEliteButton").Visible = hasAdoptedTradition
		self.parent.GetControl("LegalismButton").Visible = hasAdoptedTradition
		self.parent.GetControl("MonarchyButton").Visible = hasAdoptedTradition
		self.parent.GetControl("OligarchyButton").Visible = hasAdoptedTradition

		""" Liberty """
		hasAdoptedLiberty = self.HasAdopted("Liberty")
		self.parent.GetControl("AdoptLibertyButton").Visible = not hasAdoptedLiberty
		self.parent.GetControl("CollectiveRuleButton").Visible = hasAdoptedLiberty
		self.parent.GetControl("CitizenshipButton").Visible = hasAdoptedLiberty
		self.parent.GetControl("MeritocracyButton").Visible = hasAdoptedLiberty
		self.parent.GetControl("RepresentationButton").Visible = hasAdoptedLiberty
		self.parent.GetControl("RepublicButton").Visible = hasAdoptedLiberty

		""" Honor """
		hasAdoptedHonor = self.HasAdopted("Honor")
		canAdoptHonor = self.CanAdopt("Honor")
		self.parent.GetControl("AdoptHonorButton").Visible = not hasAdoptedHonor
		self.parent.GetControl("AdoptHonorButton").Enabled = canAdoptHonor
		self.parent.GetControl("WarriorCodeButton").Visible = hasAdoptedHonor
		self.parent.GetControl("DisciplineButton").Visible = hasAdoptedHonor
		self.parent.GetControl("MilitaryTraditionButton").Visible = hasAdoptedHonor
		self.parent.GetControl("MilitaryCasteButton").Visible = hasAdoptedHonor
		self.parent.GetControl("ProfessionalArmyButton").Visible = hasAdoptedHonor

		""" Piety """
		hasAdoptedPiety = self.HasAdopted("Piety")
		canAdoptPiety = self.CanAdopt("Piety")
		self.parent.GetControl("AdoptPietyButton").Visible = not hasAdoptedPiety
		self.parent.GetControl("AdoptPietyButton").Enabled = canAdoptPiety

		""" Patronage """
		hasAdoptedPatronage = self.HasAdopted("Patronage")
		canAdoptPatronage = self.CanAdopt("Patronage")
		self.parent.GetControl("AdoptPatronageButton").Visible = not hasAdoptedPatronage
		self.parent.GetControl("AdoptPatronageButton").Enabled = canAdoptPatronage

		""" Update Progress """
		self.parent.GetControl("Progress").Range = game.Human.CultureNeededForChange
		self.parent.GetControl("Progress").Value = game.Human.Culture

""" finally create the instance """
window = Window()