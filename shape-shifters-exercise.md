# Exercises: Shape Shifters
In this exercise we will learn how to deal with changing message schemas in an event-driven architecture. Every exercise is about a realistic scenario in which something changes in the message schema. Your task is to make the necessary changes to the handler of the message to support the change.

## Context & Assumptions
Assume that you are working in a system (the API) that is already in production and that messages are constantly flowing through the system. Messages (events) are sent to it by a different application via ServiceBus. 

You do not have the option of updating the producer of the messages at the same time as this API.

All code you need for the exercises below can be found in the ShapeShifters folder in the API project.

## 1) Remove support of BSN
The API has a handler for processing patient registrations: PatientRegisteredHandler. The PatientRegistered message has a property BSN that contains the citizen service number of the patient. Due to privacy concerns, the decision has been made to stop using this property and remove it from the message schema.
Your task is to make the required changes to the PatientRegisteredHandler to support this change.

Note: for this exercise you do not have to think about existing data in storage.

## 2) Add support for currencies in order flow
The API has a handler for saving placed orders: OrderPlacedHandler. Until now, it was assumed that all orders are paid in euros, but a new feature will be added soon that allows customers to pay in different currencies. 

Your task is to make the required changes to the OrderPlacedHandler to support this change.

Note: an enum Currency has already been added to the project for you.

## 3) Local to UTC
Our API also has a handler for processing scheduled appointments: AppointmentScheduledHandler. The ScheduledAt property of the AppointmentScheduled message is currently interpreted as a local time, but in the future we want our producers so send this property as a UTC time. 

Your task is to make the required changes to the AppointmentScheduledHandler to support this change.

## 4) Change in integration with 3rd party system
The API has an integration with a 3rd party that currently sends us PolicyInformationEvent messages. They are processed in InsurancePolicyEventsHandler. The integration will soon be changed. They will start sending the information in two separate messages: PolicyCreated and PremiumCalculated.

Your task is to make the required changes to the InsurancePolicyEventsHandler to support this change. Make sure to support the old and the new messages.